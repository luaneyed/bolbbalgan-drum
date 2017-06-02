using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using Kinect = Windows.Kinect;

public class _StickRecognizer : MonoBehaviour {
    public BodySourceManager bodyManager;
    public DepthSourceManager depthManager;

    private GameObject leftHandCube;
    private GameObject rightHandCube;

    private Kinect.KinectSensor sensor;
    private Kinect.CoordinateMapper mapper;
    private Kinect.FrameDescription depthFrameInfo;

    private Texture2D depthImg;
    private Dictionary<Kinect.JointType, Kinect.Joint> joints;
    private ushort[] depthDataCache = null;
    private bool[] stickIndexCache = null;
    Kinect.Body[] bodyData;


    private struct index
    {
        int x, y;
    }

    void Start () {
        leftHandCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rightHandCube = GameObject.CreatePrimitive(PrimitiveType.Cube);

        sensor = Kinect.KinectSensor.GetDefault();
        mapper = sensor.CoordinateMapper;
        depthFrameInfo = sensor.DepthFrameSource.FrameDescription;
        depthImg = new Texture2D(depthFrameInfo.Width, depthFrameInfo.Height, TextureFormat.RGB24, false);
    }
	
	void Update () {
        // Track Body
        bodyData = bodyManager.GetData();
        if (bodyData == null) return;

        // Get Joint
        foreach (Kinect.Body b in bodyData)
        {
            if (b.IsTracked)
            {
                joints = b.Joints;
                break;
            }
        }
        if (joints == null) return;

        // Get Depth
        ushort[] depthData = depthManager.GetData();

        //  Calculate Sticks Index
        bool[] stickIndex = getStickIndex(depthData);

        bool useCache = stickIndex == null;
        /*
        if (useCache)
        {

            if (depthDataCache != null)
                depthData = (ushort[])depthDataCache.Clone();

            if (stickIndexCache != null)
                stickIndex = (bool[])stickIndexCache.Clone();
        }
        */

        if (!useCache)
            printDepthImg(depthData, stickIndex);
    }

    bool[] getStickIndex(ushort[] depthData)
    {
        bool[] stickIndex = new bool[depthData.Length];
        for (int i = 0; i < depthData.Length; i++)
            stickIndex[i] = false;

        Kinect.CameraSpacePoint leftHandCameraPoint = joints[Kinect.JointType.HandLeft].Position;
        Kinect.DepthSpacePoint leftHandDepthPoint = mapper.MapCameraPointToDepthSpace(leftHandCameraPoint);
        Kinect.CameraSpacePoint rightHandCameraPoint = joints[Kinect.JointType.HandRight].Position;
        Kinect.DepthSpacePoint rightHandDepthPoint = mapper.MapCameraPointToDepthSpace(rightHandCameraPoint);

        depthData = (ushort[])depthData.Clone();
        cutDepthData(depthData, (int)Mathf.Min(leftHandDepthPoint.Y, rightHandDepthPoint.Y) + 300, (int)(Mathf.Max(leftHandCameraPoint.Z, rightHandCameraPoint.Z) * 1000));

        //  left hand
        int leftStickEndInd = getStickEnd(depthData, leftHandCameraPoint, leftHandDepthPoint);
        bool useCache = leftStickEndInd == 0;
        if (useCache)
            return null;
        int rightStickEndInd = getStickEnd(depthData, rightHandCameraPoint, rightHandDepthPoint);
        useCache |= rightStickEndInd == 0;
        if (useCache)
            return null;

        depthDataCache = (ushort[])depthData.Clone();

        addStickIndex(stickIndex, depthData, leftHandDepthPoint, leftStickEndInd);

        //  right hand
        addStickIndex(stickIndex, depthData, rightHandDepthPoint, rightStickEndInd);
       
        stickIndexCache = (bool[])stickIndex.Clone();
            

        return stickIndex;
    }

    void addStickIndex(bool[] stickIndex, ushort[] depthData, Kinect.DepthSpacePoint hand, int stickEndInd)
    {
        int handX = (int)hand.X;
        int handY = (int)hand.Y;
        int stickEndX = stickEndInd % depthFrameInfo.Width;
        int stickEndY = stickEndInd / depthFrameInfo.Width;

        float slope = (float)(handY - stickEndY) / (float)(handX - stickEndX);
        int direction = stickEndX > handX ? 1 : -1;
        for (int x = handX; x != stickEndX; x += direction)
        {
            int y = (int)(handY + (x - handX) * slope);
            if (pos2ind(x, y) < depthData.Length)
                stickIndex[pos2ind(x, y)] = true;
        }
    }

    void cutDepthData(ushort[] depthData, int yLimit, int depthLimit)
    {
        int max = Mathf.Min(depthFrameInfo.Width * yLimit, depthData.Length);
        for (int i = 0; i < max; i++)
        {
            if (depthData[i] > depthLimit)
                depthData[i] = 0;
            else if(depthData[i] != 0)
                depthData[i] = (ushort)((depthData[i] << 16) / depthLimit);
        }

        for (int i = max; i < depthData.Length; i++)
            depthData[i] = 0;
    }

    int getStickEnd(ushort[] depthData, Kinect.CameraSpacePoint handCameraPoint, Kinect.DepthSpacePoint handDepthPoint)
    {
        //depthData = (ushort[])depthData.Clone();
        //cutDepthData(depthData, (int)handDepthPoint.Y + 100, (int)(handCameraPoint.Z * 1000));
        
        return DFS(depthData, (int)handDepthPoint.X, (int)handDepthPoint.Y);
    }

    public int pos2ind(int x, int y)
    {
        return x + y * depthFrameInfo.Width;
    }

    public int DFS(ushort[] depthData, int x, int y)
    {
        HashSet<int> visited = new HashSet<int>();
        float max = 0;
        int maxX = 0, maxY = 0;
        
        DFS_helper(depthData, pos2ind(x, y), ref max, ref maxX, ref maxY, ref visited, x, y);

        return pos2ind(maxX, maxY);
    }

    public void DFS_helper(ushort[] depthData, int start, ref float max, ref int maxX, ref int maxY, ref HashSet<int> visited, int handIndX, int handIndY)
    {
        visited.Add(start);

        int x = start % depthFrameInfo.Width;
        int y = start / depthFrameInfo.Width;

        float len = length(x, y, handIndX, handIndY);
        if (len > max)
        {
            max = len;
            maxX = x;
            maxY = y;
        }

        int walk = 1;
        int threshould = 300;
        ushort curDep = depthData[pos2ind(x, y)];

        System.Func<int, int, bool> depthCondition = (newX, newY) => { return depthData[pos2ind(newX, newY)] != 0 && (Mathf.Abs(depthData[pos2ind(newX, newY)] - curDep) < threshould); };

        if (x + walk < depthFrameInfo.Width && depthCondition(x + walk, y) && !visited.Contains(pos2ind(x + walk, y)))
            DFS_helper(depthData, pos2ind(x + walk, y), ref max, ref maxX, ref maxY, ref visited, handIndX, handIndY);
        
        if (x >= walk && depthCondition(x - walk, y) && !visited.Contains(pos2ind(x - walk, y)))
            DFS_helper(depthData, pos2ind(x - walk, y), ref max, ref maxX, ref maxY, ref visited, handIndX, handIndY);

        if (y + walk < depthFrameInfo.Height && depthCondition(x, y + walk) && !visited.Contains(pos2ind(x, y + walk)))
            DFS_helper(depthData, pos2ind(x, y + walk), ref max, ref maxX, ref maxY, ref visited, handIndX, handIndY);

        if (y >= walk && depthCondition(x, y - walk) && !visited.Contains(pos2ind(x, y - walk)))
            DFS_helper(depthData, pos2ind(x, y - walk), ref max, ref maxX, ref maxY, ref visited, handIndX, handIndY);
            
    }

    public static float length(int x1, int y1, int x2, int y2)
    {
        return (x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2);
    }

    private float jointDistance(Kinect.Joint a, Kinect.Joint b)
    {
        Kinect.CameraSpacePoint _a = a.Position;
        Kinect.CameraSpacePoint _b = b.Position;

        float dx = _a.X - _b.X;
        float dy = _a.Y - _b.Y;
        float dz = _a.Z - _b.Z;

        return Mathf.Sqrt(dx * dx + dy * dy + dz * dz);
    }

    void printDepthImg(ushort[] depthData, bool[] stickIndex)
    {
        
        for (int i = 0; i < depthData.Length; i ++)
        {
            if (!stickIndex[i])
            {
                depthData[i] = (ushort)((depthData[i] << 16) / 2000);
            }
            else
            {
                depthData[i] = 65280;
            }
        }
        

        byte[] pixel = new byte[3* depthData.Length];

        for(int i = 0; i < depthData.Length; i++)
        {
            if (depthData[i] == 65280)
            {
                pixel[3 * i + 1] = 255;
            }
            else
            {
                byte b = (byte)(depthData[i] >> 8);
                //https://msdn.microsoft.com/en-us/library/hh973078.aspx
                //pixel[3 * i] = ;
                pixel[3 * i] = (byte)(255 - b);
                pixel[3 * i + 2] = b;
            }
        }
        depthImg.LoadRawTextureData(pixel);
        depthImg.Apply();
        GetComponent<Renderer>().material.mainTexture = depthImg;
        /*
        if ((int) Time.time % 5 == 0)
        {
            byte[] jpg = depthImg.EncodeToJPG();
            File.WriteAllBytes(Application.dataPath + "/" + ((int)(Time.time/5)).ToString() + ".jpg", jpg);
        }
        */
    }
}