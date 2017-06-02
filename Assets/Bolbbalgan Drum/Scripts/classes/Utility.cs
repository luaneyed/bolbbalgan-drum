using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Kinect = Windows.Kinect;

public class Utility {
    public static Vector3 CameraSpacePointToWorld(Kinect.CameraSpacePoint p)
    {
        return new Vector3(-10 * p.X, 10 * p.Y, 13 * p.Z);
    }
    
    public static float XZdistance(Vector3 a, Vector3 b)
    {
        a.y = 0;
        b.y = 0;
        return Vector3.Distance(a, b);
    }
}
