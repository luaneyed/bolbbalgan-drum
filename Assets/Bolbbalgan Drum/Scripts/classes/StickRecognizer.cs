using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Kinect = Windows.Kinect;

public class StickRecognizer {
    private int _Width, _Height;
    private Kinect.CameraSpacePoint _LeftTipCache, _RightTipCache;
    private int _LeftCacheElapsedFrame, _RightCacheElapsedFrame;

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

        debugPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
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
            if (tempDepthData[i] > (tempDepthData[Pos2Idx((int)leftHandDepthPoint.X, (int)leftHandDepthPoint.Y)] + 100))
            {
                tempDepthData[i] = 0;
            }
        }

        //int leftStickEndIdx = GetStickEnd(manager.DepthData, leftHandCameraPoint, leftHandDepthPoint);
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
            if (tempDepthData[i] > (tempDepthData[Pos2Idx((int)rightHandDepthPoint.X, (int)rightHandDepthPoint.Y)] + 100))
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
            /*
            colorData[3 * (idx + 1) + 1] = 255;
            colorData[3 * (idx - 1) + 1] = 255;
            colorData[3 * (idx + _Width) + 1] = 255;
            colorData[3 * (idx - _Width) + 1] = 255;
            colorData[3 * (idx + _Width + 1) + 1] = 255;
            colorData[3 * (idx + _Width - 1) + 1] = 255;
            colorData[3 * (idx - _Width + 1) + 1] = 255;
            colorData[3 * (idx - _Width - 1) + 1] = 255;
            */

        };
        colorRed(leftIdx);
        colorRed(rightIdx);

        debugTexture.LoadRawTextureData(colorData);
        debugTexture.Apply();
        debugPlane.GetComponent<Renderer>().material.mainTexture = debugTexture;
    }

    private int GetStickEnd(ushort[] depthData, Kinect.CameraSpacePoint handCameraPoint, Kinect.DepthSpacePoint handDepthPoint, ref byte[] colorData)
    {
        return DFS(depthData, (int)handDepthPoint.X, (int)handDepthPoint.Y, ref colorData);
    }

    private int DFS(ushort[] depthData, int x, int y, ref byte[] colorData)
    {
        HashSet<int> visited = new HashSet<int>();
        float max = 0;
        int maxX = 0, maxY = 0;

        DFS_helper(depthData, Pos2Idx(x, y), ref max, ref maxX, ref maxY, ref visited, x, y);

        
        /*for (int i = 0; i < depthData.Length; i ++)
        {
            if (visited.Contains(i))
            {
                colorData[3 * i] = 255;
            }
        }
        debugTexture.LoadRawTextureData(colorData);
        debugTexture.Apply();
        debugPlane.GetComponent<Renderer>().material.mainTexture = debugTexture;*/

        return Pos2Idx(maxX, maxY);
    }

    private void DFS_helper(ushort[] depthData, int start, ref float max, ref int maxX, ref int maxY, ref HashSet<int> visited, int handIndX, int handIndY)
    {
        visited.Add(start);

        ushort curDep = depthData[start];
        if (curDep == 0) {
            return;
        }

        int x = start % _Width;
        int y = start / _Width;

        float len = Length(x, y, handIndX, handIndY);

        /*if (len > 700) {   //  (x, y) should be out of hand
            float slope = (float)(handIndY - y) / (x - handIndX);
            float angle = Mathf.Atan(slope);
            float distance = 35;
            int dx = (int)(distance * Mathf.Cos(angle));
            int dy = (int)(distance * Mathf.Sin(angle));
            int outThreshold = 600;
            System.Func<int, int, bool> isBadNeighbor = (X, Y) =>
            {
                return validateDepthPosition(X, Y) && Mathf.Abs(depthData[Pos2Idx(X, Y)] - curDep) <= outThreshold;
            };
            if (isBadNeighbor(x + dx, y + dy) || isBadNeighbor(x - dx, y - dy)) {
                return;
            }
        }*/

        if (len > max)
        {
            max = len;
            maxX = x;
            maxY = y;
        }

        int walk = 1;

        //int inThreshold = len < 800 ? 1500 : 300;
        //System.Func<int, int, bool> depthCondition = (X, Y) => { return Mathf.Abs(depthData[Pos2Idx(X, Y)] - curDep) < inThreshold; };
        System.Func<int, int, bool> depthCondition = (X, Y) => { return true; };

        int newX = x + walk, newY = y;
        if (validateDepthPosition(newX, newY) && depthCondition(newX, newY) && !visited.Contains(Pos2Idx(newX, newY)))
            DFS_helper(depthData, Pos2Idx(newX, newY), ref max, ref maxX, ref maxY, ref visited, handIndX, handIndY);

        newX = x - walk;
        if (validateDepthPosition(newX, newY) && depthCondition(newX, newY) && !visited.Contains(Pos2Idx(newX, newY)))
            DFS_helper(depthData, Pos2Idx(newX, newY), ref max, ref maxX, ref maxY, ref visited, handIndX, handIndY);

        newX = x;
        newY = y + walk;
        if (validateDepthPosition(newX, newY) && depthCondition(newX, newY) && !visited.Contains(Pos2Idx(newX, newY)))
            DFS_helper(depthData, Pos2Idx(newX, newY), ref max, ref maxX, ref maxY, ref visited, handIndX, handIndY);

        newY = y - walk;
        if (validateDepthPosition(newX, newY) && depthCondition(newX, newY) && !visited.Contains(Pos2Idx(newX, newY)))
            DFS_helper(depthData, Pos2Idx(newX, newY), ref max, ref maxX, ref maxY, ref visited, handIndX, handIndY);
    }

    private bool validateDepthPosition(int x, int y) {
        return x >= 0 && x < _Width && y >= 0 && y < _Height;
    }

    private int Pos2Idx(int x, int y)
    {
        return x + y * _Width;
    }

    private float Length(int x1, int y1, int x2, int y2)
    {
        return (x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2);
    }
}
