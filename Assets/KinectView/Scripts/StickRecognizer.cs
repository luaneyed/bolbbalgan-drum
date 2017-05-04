using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using Kinect = Windows.Kinect;

public class StickRecognizer : MonoBehaviour {
    public BodySourceManager bodyManager;
    public DepthSourceManager depthManager;

    private GameObject leftHandCube;
    private GameObject rightHandCube;

    private Kinect.DepthSpacePoint[] keyPoints;

    private Kinect.KinectSensor sensor;
    private Kinect.CoordinateMapper mapper;
    private Kinect.FrameDescription depthFrameInfo;

    private Texture2D depthImg;
    private Dictionary<Kinect.JointType, Kinect.Joint> joints;
    private ushort[] depthData;
    private bool[,] imageArray;
    Kinect.Body[] bodyData;

    void Start () {
        leftHandCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rightHandCube = GameObject.CreatePrimitive(PrimitiveType.Cube);

        sensor = Kinect.KinectSensor.GetDefault();
        mapper = sensor.CoordinateMapper;
        depthFrameInfo = sensor.DepthFrameSource.FrameDescription;
        depthImg = new Texture2D(depthFrameInfo.Width, depthFrameInfo.Height, TextureFormat.RGB24, false);
        keyPoints = new Kinect.DepthSpacePoint[3];
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
        depthData = depthManager.GetData();
        //imageArray = new bool[depthFrameInfo.Height, depthFrameInfo.Width];

        cropStickRange();
        printDepthImg();
    }

    void cropStickRange()
    {
        Kinect.Joint left = joints[Kinect.JointType.HandLeft];
        Kinect.Joint right = joints[Kinect.JointType.HandRight];

        float depthLimit = Mathf.Max(left.Position.Z, right.Position.Z) * 1000;
        Kinect.DepthSpacePoint pLeft = mapper.MapCameraPointToDepthSpace(left.Position);
        Kinect.DepthSpacePoint pRight = mapper.MapCameraPointToDepthSpace(right.Position);
        float yLimit = Mathf.Max(pLeft.Y, pRight.Y) + 100;

        for (int i = 0; i < depthData.Length; i++)
        {
            if(depthData[i] > depthLimit || i > depthFrameInfo.Width * yLimit)
            {
                depthData[i] = 0;
                //imageArray[i / depthFrameInfo.Width, i % depthFrameInfo.Width] = false;
            } else
            {
                depthData[i] = (ushort) ((depthData[i]<<16)/depthLimit);
                //imageArray[i / depthFrameInfo.Width, i % depthFrameInfo.Width] = true;
            }
        }
    }

    public static bool[,] ZhangSuenThinning(bool[,] s)
    {
        //bool[][] temp = ArrayClone(s);  // make a deep copy to start.. 
        bool[,] temp = (bool[,])s.Clone();
        int count = 0;
        do  // the missing iteration
        {
            count = step(1, temp, s);
            temp = (bool[,])s.Clone();      // ..and on each..
            count += step(2, temp, s);
            temp = (bool[,])s.Clone();      // ..call!
        }
        while (count > 0);

        return s;
    }

    static int step(int stepNo, bool[,] temp, bool[,] s)
    {
        int count = 0;

        for (int a = 1; a < temp.Length - 1; a++)
        {
            for (int b = 1; b < temp.GetLength(0) - 1; b++)
            {
                if (SuenThinningAlg(a, b, temp, stepNo == 2))
                {
                    // still changes happening?
                    if (s[a,b]) count++;
                    s[a,b] = false;
                }
            }
        }
        return count;
    }

    static bool SuenThinningAlg(int x, int y, bool[,] s, bool even)
    {
        bool p2 = s[x, y - 1];
        Debug.Log(y);
        bool p3 = s[x + 1, y - 1];
        bool p4 = s[x + 1, y];
        bool p5 = s[x + 1, y + 1];
        bool p6 = s[x, y + 1];
        bool p7 = s[x - 1, y + 1];
        bool p8 = s[x - 1, y];
        bool p9 = s[x - 1, y - 1];


        int bp1 = NumberOfNonZeroNeighbors(x, y, s);
        if (bp1 >= 2 && bp1 <= 6) //2nd condition
        {
            if (NumberOfZeroToOneTransitionFromP9(x, y, s) == 1)
            {
                if (even)
                {
                    if (!((p2 && p4) && p8))
                    {
                        if (!((p2 && p6) && p8))
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    if (!((p2 && p4) && p6))
                    {
                        if (!((p4 && p6) && p8))
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    static int NumberOfZeroToOneTransitionFromP9(int x, int y, bool[,] s)
    {
        bool p2 = s[x, y - 1];
        bool p3 = s[x + 1, y - 1];
        bool p4 = s[x + 1, y];
        bool p5 = s[x + 1, y + 1];
        bool p6 = s[x, y + 1];
        bool p7 = s[x - 1, y + 1];
        bool p8 = s[x - 1, y];
        bool p9 = s[x - 1, y - 1];

        int A = System.Convert.ToInt32((!p2 && p3)) + System.Convert.ToInt32((!p3 && p4)) +
                System.Convert.ToInt32((!p4 && p5)) + System.Convert.ToInt32((!p5 && p6)) +
                System.Convert.ToInt32((!p6 && p7)) + System.Convert.ToInt32((!p7 && p8)) +
                System.Convert.ToInt32((!p8 && p9)) + System.Convert.ToInt32((!p9 && p2));
        return A;
    }
    static int NumberOfNonZeroNeighbors(int x, int y, bool[,] s)
    {
        int count = 0;
        if (s[x - 1, y]) count++;
        if (s[x - 1, y + 1]) count++;
        if (s[x - 1, y - 1]) count++;
        if (s[x, y + 1]) count++;
        if (s[x, y - 1]) count++;
        if (s[x + 1, y]) count++;
        if (s[x + 1, y + 1]) count++;
        if (s[x + 1, y - 1]) count++;
        return count;
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

    void printDepthImg()
    {
        byte[] pixel = new byte[3* depthData.Length];

        for(int i = 0; i < depthData.Length; i++)
        {
            byte b = (byte) (depthData[i] >> 8);
            //https://msdn.microsoft.com/en-us/library/hh973078.aspx
            pixel[3 * i] = (byte) (256 - b);
            pixel[3 * i + 2] = b;
        }
        depthImg.LoadRawTextureData(pixel);
        depthImg.Apply();
        GetComponent<Renderer>().material.mainTexture = depthImg;

        if ((int) Time.time % 5 == 0)
        {
            byte[] jpg = depthImg.EncodeToJPG();
            File.WriteAllBytes(Application.dataPath + "/" + ((int)(Time.time/5)).ToString() + ".jpg", jpg);
        }
    }
}