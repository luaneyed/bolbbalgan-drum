using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Kinect = Windows.Kinect;

public class StickRecognizer {
    private int _Width, _Height;
    private Kinect.CameraSpacePoint _LeftTipCache, _RightTipCache;
    private int _LeftCacheElapsedFrame, _RightCacheElapsedFrame;

    public StickRecognizer(KinectManager manager)
    {
        _Width = manager.DepthFrameDesc.Width;
        _Height = manager.DepthFrameDesc.Height;
        _LeftTipCache = new Kinect.CameraSpacePoint();
        _RightTipCache = new Kinect.CameraSpacePoint();
        _LeftCacheElapsedFrame = 10;
        _RightCacheElapsedFrame = 10;
    }

    public void FindTip(KinectManager manager,
        out Kinect.CameraSpacePoint leftTipCameraPoint, out Kinect.CameraSpacePoint rightTipCameraPoint,
        out bool leftStatus, out bool rightStatus)
    {
        Kinect.CameraSpacePoint leftHandCameraPoint = manager.JointData[Kinect.JointType.HandLeft].Position;
        Kinect.DepthSpacePoint leftHandDepthPoint = manager.Mapper.MapCameraPointToDepthSpace(leftHandCameraPoint);
        Kinect.CameraSpacePoint rightHandCameraPoint = manager.JointData[Kinect.JointType.HandRight].Position;
        Kinect.DepthSpacePoint rightHandDepthPoint = manager.Mapper.MapCameraPointToDepthSpace(rightHandCameraPoint);

        int leftStickEndIdx = GetStickEnd(manager.DepthData, leftHandCameraPoint, leftHandDepthPoint);
        if(leftStickEndIdx == 0)
        {
            leftTipCameraPoint = _LeftTipCache;
            if (_LeftCacheElapsedFrame == 10)
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

        int rightStickEndIdx = GetStickEnd(manager.DepthData, rightHandCameraPoint, rightHandDepthPoint);
        if (rightStickEndIdx == 0)
        {
            rightTipCameraPoint = _RightTipCache;
            if (_RightCacheElapsedFrame == 10)
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
    }

    private int GetStickEnd(ushort[] depthData, Kinect.CameraSpacePoint handCameraPoint, Kinect.DepthSpacePoint handDepthPoint)
    {
        return DFS(depthData, (int)handDepthPoint.X, (int)handDepthPoint.Y);
    }

    private int DFS(ushort[] depthData, int x, int y)
    {
        HashSet<int> visited = new HashSet<int>();
        float max = 0;
        int maxX = 0, maxY = 0;

        DFS_helper(depthData, Pos2Idx(x, y), ref max, ref maxX, ref maxY, ref visited, x, y);

        return Pos2Idx(maxX, maxY);
    }

    private void DFS_helper(ushort[] depthData, int start, ref float max, ref int maxX, ref int maxY, ref HashSet<int> visited, int handIndX, int handIndY)
    {
        visited.Add(start);

        int x = start % _Width;
        int y = start / _Width;

        float len = Length(x, y, handIndX, handIndY);
        if (len > max)
        {
            max = len;
            maxX = x;
            maxY = y;
        }

        int walk = 1;
        int threshould = 300;
        ushort curDep = depthData[Pos2Idx(x, y)];

        System.Func<int, int, bool> depthCondition = (newX, newY) => { return depthData[Pos2Idx(newX, newY)] != 0 && (Mathf.Abs(depthData[Pos2Idx(newX, newY)] - curDep) < threshould); };

        if (x + walk < _Width && depthCondition(x + walk, y) && !visited.Contains(Pos2Idx(x + walk, y)))
            DFS_helper(depthData, Pos2Idx(x + walk, y), ref max, ref maxX, ref maxY, ref visited, handIndX, handIndY);

        if (x >= walk && depthCondition(x - walk, y) && !visited.Contains(Pos2Idx(x - walk, y)))
            DFS_helper(depthData, Pos2Idx(x - walk, y), ref max, ref maxX, ref maxY, ref visited, handIndX, handIndY);

        if (y + walk < _Height && depthCondition(x, y + walk) && !visited.Contains(Pos2Idx(x, y + walk)))
            DFS_helper(depthData, Pos2Idx(x, y + walk), ref max, ref maxX, ref maxY, ref visited, handIndX, handIndY);

        if (y >= walk && depthCondition(x, y - walk) && !visited.Contains(Pos2Idx(x, y - walk)))
            DFS_helper(depthData, Pos2Idx(x, y - walk), ref max, ref maxX, ref maxY, ref visited, handIndX, handIndY);
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
