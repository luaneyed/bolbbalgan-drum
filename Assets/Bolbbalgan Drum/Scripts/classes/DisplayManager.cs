using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Kinect = Windows.Kinect;

public class DisplayManager
{
    GameObject leftStick, rightStick;

    public DisplayManager()
    {
        leftStick = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        rightStick = GameObject.CreatePrimitive(PrimitiveType.Cylinder);

        leftStick.transform.localScale = new Vector3(1, 10, 1);
        rightStick.transform.localScale = new Vector3(1, 10, 1);
        /*
        leftHand = (GameObject) GameObject.Instantiate(Resources.Load("hand"));
        rightHand = (GameObject)GameObject.Instantiate(Resources.Load("hand"));
        

        Vector3 scale = rightHand.transform.localScale;
        scale.x = -scale.x;
        rightHand.transform.localScale = scale;
        */
    }

    public void DrawBody(KinectManager manager)
    {
        leftStick.transform.position = JointToVector3(manager.JointData[Kinect.JointType.HandLeft]);
        rightStick.transform.position = JointToVector3(manager.JointData[Kinect.JointType.HandRight]);

        leftStick.transform.rotation = JointOrientationToQuaternion(manager.JointOriendationData[Kinect.JointType.WristLeft]);
        rightStick.transform.rotation = JointOrientationToQuaternion(manager.JointOriendationData[Kinect.JointType.WristRight]);
    }

    public void DisplayPlayer(KinectManager manager, Kinect.CameraSpacePoint leftTip, Kinect.CameraSpacePoint rightTip)
    {
        Kinect.CameraSpacePoint leftHand = manager.JointData[Kinect.JointType.HandLeft].Position;
        Kinect.CameraSpacePoint rightHand = manager.JointData[Kinect.JointType.HandRight].Position;
    }

    private Vector3 JointToVector3(Kinect.Joint joint)
    {
        Vector3 vec;
        vec.x = 100 * joint.Position.X;
        vec.y = 100 * joint.Position.Y;
        vec.z = joint.Position.Z;
        return vec;
    } 

    private Quaternion JointOrientationToQuaternion(Kinect.JointOrientation ori)
    {
        Quaternion q;
        q.x = ori.Orientation.X;
        q.y = ori.Orientation.Y;
        q.z = ori.Orientation.Z;
        q.w = ori.Orientation.W;
        return q;
    }
}
