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

        ushort curDep = depthData[start];
        if (curDep == 0) {
            return
        }

        int x = start % _Width;
        int y = start / _Width;

        float len = Length(x, y, handIndX, handIndY);
        int threshold = 300;

        if (len > 2500) {   //  (x, y) should be out of hand
            float slope = (float)(handIndY - y) / (x - handIndX);
            float angle = Mathf.atan(slope);
            float distance = 15
            float dx = (int)(distance * Mathf.cos(angle));
            float dy = (int)(distance * Mathf.sin(angle));

            System.Func<int, int, bool> isBadNeighbor = (x, y) => {
                return validateDepthPosition(x, y) && Mathf.Abs(depthData[Pos2Idx(x, y)] - curDep) <= threshold
            }
            if (isBadNeighbor(x + dx, y + dy) || isBadNeighbor(x - dx, y - dy)) {
                return
            }
        }

        if (len > max)
        {
            max = len;
            maxX = x;
            maxY = y;
        }

        int walk = 1;

        System.Func<int, int, bool> depthCondition = (newX, newY) => { return Mathf.Abs(depthData[Pos2Idx(newX, newY)] - curDep) < threshold; };
        System.Action<int, int> traverse = (newX, newY) => {
            if (validateDepthPosition(newX, newY) && depthCondition(newX, newY) && !visited.Contains(Pos2Idx(newX, newY)))
                DFS_helper(depthData, Pos2Idx(newX, newY), ref max, ref maxX, ref maxY, ref visited, handIndX, handIndY);
        }

        traverse(x + walk, y);
        traverse(x - walk, y);
        traverse(x, y + walk);
        traverse(x, y - walk);
    }

    private bool validateDepthPosition(int x, int y) {
        return x >= 0 && x < _Width && y >0 0 && y <= _Height
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
