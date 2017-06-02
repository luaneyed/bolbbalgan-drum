using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Kinect = Windows.Kinect;

struct DFS_Datas {
    public ushort[] depthData;
    public float max;
    public int maxX;
    public int maxY;
    public HashSet<int> visited;
    public int handIndX;
    public int handIndY;
}

public class StickRecognizer {
    private int _Width, _Height;
    private Kinect.CameraSpacePoint _LeftTipCache, _RightTipCache;
    private int _LeftCacheElapsedFrame, _RightCacheElapsedFrame;
    private Kinect.CoordinateMapper _Mapper;

    private GameObject debugPlane;
    private Texture2D debugTexture;

    public StickRecognizer(KinectManager manager)
    {
        _Width = manager.DepthFrameDesc.Width;
        _Height = manager.DepthFrameDesc.Height;
        _LeftTipCache = new Kinect.CameraSpacePoint();
        _RightTipCache = new Kinect.CameraSpacePoint();
        _LeftCacheElapsedFrame = 10;
        _RightCacheElapsedFrame = 10;
        _Mapper = manager.Mapper;

        debugPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        debugPlane.transform.Translate(-10, 2, 0);
        debugPlane.transform.Rotate(-90, 180, 0);
        debugTexture = new Texture2D(manager.DepthFrameDesc.Width, manager.DepthFrameDesc.Height, TextureFormat.RGB24, false);
    }

    public void FindTip(KinectManager manager,
        out Kinect.CameraSpacePoint leftTipCameraPoint, out Kinect.CameraSpacePoint rightTipCameraPoint,
        out bool leftStatus, out bool rightStatus)
    {
        Kinect.CameraSpacePoint leftHandCameraPoint = manager.JointData[Kinect.JointType.HandLeft].Position;
        Kinect.DepthSpacePoint leftHandDepthPoint = manager.Mapper.MapCameraPointToDepthSpace(leftHandCameraPoint);
        Kinect.CameraSpacePoint rightHandCameraPoint = manager.JointData[Kinect.JointType.HandRight].Position;
        Kinect.DepthSpacePoint rightHandDepthPoint = manager.Mapper.MapCameraPointToDepthSpace(rightHandCameraPoint);
        byte[] colorData = new byte[3 * manager.DepthData.Length];

        ushort[] tempDepthData = new ushort[manager.DepthData.Length];
        tempDepthData = (ushort[])manager.DepthData.Clone();
 

        for(int i = 0; i < tempDepthData.Length; i++)
        {
            if (tempDepthData[i] > (tempDepthData[Pos2Idx((int)leftHandDepthPoint.X, (int)leftHandDepthPoint.Y)] + 30))
            {
                tempDepthData[i] = 0;
            }
        }

        int leftStickEndIdx = GetStickEnd(tempDepthData, leftHandCameraPoint, leftHandDepthPoint, ref colorData);
        if (leftStickEndIdx == 0)
        {
            leftTipCameraPoint = _LeftTipCache;
            if (_LeftCacheElapsedFrame >= 10)
            {
                leftStatus = false;
            } else
            {
                leftStatus = true;
                _LeftCacheElapsedFrame++;
            }
        } else
        {
            Kinect.DepthSpacePoint leftTipDepthPoint = new Kinect.DepthSpacePoint();
            leftTipDepthPoint.X = leftStickEndIdx % _Width;
            leftTipDepthPoint.Y = leftStickEndIdx / _Width;
            leftTipCameraPoint = manager.Mapper.MapDepthPointToCameraSpace(leftTipDepthPoint, manager.DepthData[leftStickEndIdx]);

            _LeftTipCache = leftTipCameraPoint;
            _LeftCacheElapsedFrame = 0;

            leftStatus = true;
        }

        tempDepthData = (ushort[])manager.DepthData.Clone();

        for (int i = 0; i < tempDepthData.Length; i++)
        {
            if (tempDepthData[i] > (tempDepthData[Pos2Idx((int)rightHandDepthPoint.X, (int)rightHandDepthPoint.Y)] + 30))
            {
                tempDepthData[i] = 0;
            }
        }

        //int rightStickEndIdx = GetStickEnd(manager.DepthData, rightHandCameraPoint, rightHandDepthPoint);
        int rightStickEndIdx = GetStickEnd(tempDepthData, rightHandCameraPoint, rightHandDepthPoint, ref colorData);
        if (rightStickEndIdx == 0)
        {
            rightTipCameraPoint = _RightTipCache;
            if (_RightCacheElapsedFrame >= 10)
            {
                rightStatus = false;
            }
            else
            {
                rightStatus = true;
                _RightCacheElapsedFrame++;
            }
        }
        else
        {
            Kinect.DepthSpacePoint rightTipDepthPoint = new Kinect.DepthSpacePoint();
            rightTipDepthPoint.X = rightStickEndIdx % _Width;
            rightTipDepthPoint.Y = rightStickEndIdx / _Width;
            rightTipCameraPoint = manager.Mapper.MapDepthPointToCameraSpace(rightTipDepthPoint, manager.DepthData[rightStickEndIdx]);


            _RightTipCache = rightTipCameraPoint;
            _RightCacheElapsedFrame = 0;

            rightStatus = true;
        }


        ushort[] depthData = manager.DepthData;
        DrawDebugImg(manager, leftStickEndIdx, rightStickEndIdx);
    }

    private void DrawDebugImg(KinectManager manager, int leftIdx, int rightIdx)
    {
        byte[] colorData = new byte[3*manager.DepthData.Length];
        float depthLimt = 1000*manager.JointData[Kinect.JointType.SpineBase].Position.Z;

        for(int i = 0; i < manager.DepthData.Length; i++)
        {
            if (manager.DepthData[i] < depthLimt)
                colorData[3*i] = (byte) (255*(manager.DepthData[i] / depthLimt));
        }
        
        System.Action<int> colorRed = (idx) =>
        {
            colorData[3 * idx + 1] = 255;

        };
        colorRed(leftIdx);
        colorRed(rightIdx);

        debugTexture.LoadRawTextureData(colorData);

        Kinect.DepthSpacePoint leftHand = manager.Mapper.MapCameraPointToDepthSpace(manager.JointData[Kinect.JointType.HandLeft].Position);
        Kinect.DepthSpacePoint rightHand = manager.Mapper.MapCameraPointToDepthSpace(manager.JointData[Kinect.JointType.HandRight].Position);

        DrawLine(debugTexture, leftIdx%manager.DepthFrameDesc.Width, (int) leftIdx/manager.DepthFrameDesc.Width,
                                (int) leftHand.X, (int) leftHand.Y, Color.green);
        DrawLine(debugTexture, rightIdx % manager.DepthFrameDesc.Width, (int)rightIdx / manager.DepthFrameDesc.Width,
                              (int)rightHand.X, (int)rightHand.Y, Color.green);
        debugTexture.Apply();
        debugPlane.GetComponent<Renderer>().material.mainTexture = debugTexture;
    }
    
    void DrawLine(Texture2D tex, int x0, int y0, int x1, int y1, Color col)
    {
        int dy = (int)(y1-y0);
        int dx = (int)(x1-x0);
        int stepx, stepy;
     
        if (dy < 0) {dy = -dy; stepy = -1;}
        else {stepy = 1;}
        if (dx < 0) {dx = -dx; stepx = -1;}
        else {stepx = 1;}
        dy <<= 1;
        dx <<= 1;
     
        float fraction = 0;
     
        tex.SetPixel(x0, y0, col);
        if (dx > dy) {
            fraction = dy - (dx >> 1);
            while (Mathf.Abs(x0 - x1) > 1) {
                if (fraction >= 0) {
                    y0 += stepy;
                    fraction -= dx;
                }
                x0 += stepx;
                fraction += dy;
                tex.SetPixel(x0, y0, col);
            }
        }
        else {
            fraction = dx - (dy >> 1);
            while (Mathf.Abs(y0 - y1) > 1) {
                if (fraction >= 0) {
                    x0 += stepx;
                    fraction -= dy;
                }
                y0 += stepy;
                fraction += dx;
                tex.SetPixel(x0, y0, col);
            }
        }
    }

    private int GetStickEnd(ushort[] depthData, Kinect.CameraSpacePoint handCameraPoint, Kinect.DepthSpacePoint handDepthPoint, ref byte[] colorData)
    {
        return DFS(depthData, (int)handDepthPoint.X, (int)handDepthPoint.Y, ref colorData);
    }

    private int DFS(ushort[] depthData, int x, int y, ref byte[] colorData)
    {
        DFS_Datas datas;
        datas.depthData = depthData;
        datas.max = 0;
        datas.maxX = 0;
        datas.maxY = 0;
        datas.visited = new HashSet<int>();
        datas.handIndX = x;
        datas.handIndY = y;

        DFS_helper(ref datas, Pos2Idx(x, y));

        if (datas.max == 0) {
            datas.handIndX = x + 1;
            DFS_helper(ref datas, Pos2Idx(x + 1, y));
        }
        if (datas.max == 0) {
            datas.handIndX = x - 1;
            DFS_helper(ref datas, Pos2Idx(x - 1, y));
        }
        if (datas.max == 0) {
            datas.handIndX = x;
            datas.handIndY = y +1;
            DFS_helper(ref datas, Pos2Idx(x, y + 1));
        }
        if (datas.max == 0) {
            datas.handIndY = y - 1;
            DFS_helper(ref datas, Pos2Idx(x, y - 1));
        }
        if (datas.max == 0) {
            datas.handIndX = x + 1;
            DFS_helper(ref datas, Pos2Idx(x + 1, y - 1));
        }
        if (datas.max == 0) {
            datas.handIndX = -1;
            DFS_helper(ref datas, Pos2Idx(x - 1, y - 1));
        }
        if (datas.max == 0) {
            datas.handIndY = 1;
            DFS_helper(ref datas, Pos2Idx(x - 1, y + 1));
        }
        if (datas.max == 0) {
            datas.handIndX = 1;
            DFS_helper(ref datas, Pos2Idx(x + 1, y + 1));
        }

        return Pos2Idx(datas.maxX, datas.maxY);
    }

    private void DFS_helper(ref DFS_Datas datas, int start)
    {
        datas.visited.Add(start);

        ushort curDep = datas.depthData[start];
        if (curDep == 0) {
            return;
        }

        int x = start % _Width;
        int y = start / _Width;

        Kinect.DepthSpacePoint curDSP = new Kinect.DepthSpacePoint();
        curDSP.X = x;
        curDSP.Y = y;
        Kinect.DepthSpacePoint handDSP = new Kinect.DepthSpacePoint();
        handDSP.X = datas.handIndX;
        handDSP.Y = datas.handIndY;
        float len = getDistanceWithDepthSpacePoint(curDSP, handDSP, datas.depthData);

        if (len > 0.15) {
            float slope = (float)(datas.handIndY - y) / (x - datas.handIndX);
            float angle = Mathf.Atan(slope);
            float distance = 30;
            int dx = (int)(distance * Mathf.Cos(angle));
            int dy = (int)(distance * Mathf.Sin(angle));
            int outThreshold = 400;
            ushort[] depthData = datas.depthData;
            System.Func<int, int, bool> isBadNeighbor = (X, Y) =>
            {
                return validateDepthPosition(X, Y) && Mathf.Abs(depthData[Pos2Idx(X, Y)] - curDep) <= outThreshold;
            };
            if (isBadNeighbor(x + dx, y + dy) || isBadNeighbor(x - dx, y - dy)) {
                return;
            }
        }

        if (len > datas.max)
        {
            datas.max = len;
            datas.maxX = x;
            datas.maxY = y;
        }

        int walk = 1;

        //int inThreshold = len < 800 ? 1500 : 300;
        //System.Func<int, int, bool> depthCondition = (X, Y) => { return Mathf.Abs(depthData[Pos2Idx(X, Y)] - curDep) < inThreshold; };
        System.Func<int, int, bool> depthCondition = (X, Y) => { return true; };

        int newX = x + walk, newY = y;
        if (validateDepthPosition(newX, newY) && depthCondition(newX, newY) && !datas.visited.Contains(Pos2Idx(newX, newY)))
            DFS_helper(ref datas, Pos2Idx(newX, newY));

        newX = x - walk;
        if (validateDepthPosition(newX, newY) && depthCondition(newX, newY) && !datas.visited.Contains(Pos2Idx(newX, newY)))
            DFS_helper(ref datas, Pos2Idx(newX, newY));

        newX = x;
        newY = y + walk;
        if (validateDepthPosition(newX, newY) && depthCondition(newX, newY) && !datas.visited.Contains(Pos2Idx(newX, newY)))
            DFS_helper(ref datas, Pos2Idx(newX, newY));

        newY = y - walk;
        if (validateDepthPosition(newX, newY) && depthCondition(newX, newY) && !datas.visited.Contains(Pos2Idx(newX, newY)))
            DFS_helper(ref datas, Pos2Idx(newX, newY));
    }

    private bool validateDepthPosition(int x, int y) {
        return x >= 0 && x < _Width && y >= 0 && y < _Height;
    }

    private int Pos2Idx(int x, int y)
    {
        return x + y * _Width;
    }

    private float getDistanceWithDepthSpacePoint(Kinect.DepthSpacePoint p, Kinect.DepthSpacePoint q, ushort[] depthData)
    {
        Kinect.CameraSpacePoint P = _Mapper.MapDepthPointToCameraSpace(p, depthData[Pos2Idx((int)p.X, (int)p.Y)]);
        Kinect.CameraSpacePoint Q = _Mapper.MapDepthPointToCameraSpace(q, depthData[Pos2Idx((int)q.X, (int)q.Y)]);

        return Mathf.Sqrt((P.X - Q.X) * (P.X - Q.X) + (P.Y - Q.Y) * (P.Y - Q.Y) + (P.Z - Q.Z) * (P.Z - Q.Z));
    }
}
