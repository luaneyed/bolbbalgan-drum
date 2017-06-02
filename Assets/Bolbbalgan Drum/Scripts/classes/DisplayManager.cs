using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Kinect = Windows.Kinect;

public class DisplayManager
{
    GameObject leftStick, rightStick;
    GameObject drum;
    GameObject menuDrum;
    GameObject[] parts;
    Camera MainCamera;

    public DisplayManager(Camera camera)
    {
        MainCamera = camera;
    }

    public void InitDisplay(KinectManager manager)
    {
        leftStick = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/stick"));
        rightStick = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/stick"));
        Vector3 neckPos = Utility.CameraSpacePointToWorld(manager.JointData[Kinect.JointType.Neck].Position);
        drum = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/RealDrum"));
        drum.transform.Translate(neckPos);
        DeactivateDrum();

        menuDrum = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/MenuDrum"));
        menuDrum.transform.Translate(neckPos);
        ActivateMenuDrum();

        //Camera.SetupCurrent(drum.GetComponentInChildren<Camera>());
    }

    public void ActivateDrum()
    {
        drum.SetActive(true);
        Camera.SetupCurrent(drum.GetComponentInChildren<Camera>());
    }

    public void DeactivateDrum()
    {
        drum.SetActive(false);
    }

    public void ActivateMenuDrum()
    {
        menuDrum.SetActive(true);
        Camera.SetupCurrent(menuDrum.GetComponentInChildren<Camera>());
    }

    public void DeactivateMenuDrum()
    {
        menuDrum.SetActive(false);
    }

    public void DisplayPlayer(KinectManager manager, Kinect.CameraSpacePoint leftTip, Kinect.CameraSpacePoint rightTip)
    {
        Kinect.CameraSpacePoint leftHand = manager.JointData[Kinect.JointType.HandLeft].Position;
        Kinect.CameraSpacePoint rightHand = manager.JointData[Kinect.JointType.HandRight].Position;

        leftStick.transform.position = Utility.CameraSpacePointToWorld(leftHand);
        rightStick.transform.position = Utility.CameraSpacePointToWorld(rightHand);
        leftStick.transform.LookAt(Utility.CameraSpacePointToWorld(leftTip));
        rightStick.transform.LookAt(Utility.CameraSpacePointToWorld(rightTip));
    }
}
