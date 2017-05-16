using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Kinect = Windows.Kinect;

public class DisplayManager
{
    GameObject leftStick, rightStick;

    public DisplayManager()
    {
        //leftStick = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        // rightStick = GameObject.CreatePrimitive(PrimitiveType.Cylinder);

        leftStick = (GameObject)GameObject.Instantiate(Resources.Load("Stick"));
        rightStick = (GameObject)GameObject.Instantiate(Resources.Load("Stick"));

        //leftStick.transform.localScale = new Vector3(1, 10, 1);
       // rightStick.transform.localScale = new Vector3(1, 10, 1);
        
        /*
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

    public void DisplayPlayer(KinectManager manager, Kinect.CameraSpacePoint leftTip, Kinect.CameraSpacePoint rightTip, ushort leftDepth, ushort rightDepth)
    {
        Kinect.CameraSpacePoint leftHand = manager.JointData[Kinect.JointType.HandLeft].Position;
        Kinect.CameraSpacePoint rightHand = manager.JointData[Kinect.JointType.HandRight].Position;

        leftStick.transform.position = JointToVector3(manager.JointData[Kinect.JointType.HandLeft]);
        rightStick.transform.position = JointToVector3(manager.JointData[Kinect.JointType.HandRight]);

        Vector3 leftVec = new Vector3(leftTip.X - leftHand.X, leftTip.Y - leftHand.Y, leftDepth / 1000 - leftHand.Z);
        Vector3 rightVec = new Vector3(rightTip.X - rightHand.X, rightTip.Y - rightHand.Y, rightDepth / 1000 - rightHand.Z);
        leftStick.transform.rotation = Quaternion.LookRotation(leftVec, Vector3.up);
        rightStick.transform.rotation = Quaternion.LookRotation(rightVec, Vector3.up);
    }

    private Vector3 JointToVector3(Kinect.Joint joint)
    {
        Vector3 vec;
        vec.x = joint.Position.X;
        vec.y = joint.Position.Y;
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
