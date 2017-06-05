using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Kinect = Windows.Kinect;

public class DisplayManager
{
    GameObject leftStick, rightStick;
    GameObject drum;
    GameObject menuDrum;
    GameObject customizeMenuDrum;
    GameObject[] parts;
    Camera MainCamera;

    public DisplayManager(Camera camera)
    {
        MainCamera = camera;
        drum = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/InitialDrum"));
    }

    public void ChangeDrum(State.Status nextStatus, Kinect.CameraSpacePoint position)
    {
        Vector3 neckPos = Utility.CameraSpacePointToWorld(position);
        State state = GameObject.Find("MAIN").GetComponent<main>()._State;

        if (drum != null)
        {
            GameObject.Destroy(drum);
        }

        string drumName = null;

        switch (nextStatus)
        {
            case State.Status.Menu:
                drumName = "Prefabs/MenuDrum";
                break;
            case State.Status.Playing:
                if (state.SelectedDesign == State.CustomizeDesignOption.Standard)
                    drumName = "Prefabs/RealDrum";
                else
                    drumName = "Prefabs/BolbbalganDrum";
                break;
            case State.Status.CustomizeMenu:
                drumName = "Prefabs/CustomizeMenuDrum";
                break;
            case State.Status.PositionCustomizing:
                drumName = "Prefabs/PositionCustomizeMenuDrum";
                break;
            case State.Status.PositionMoving:
                drumName = "Prefabs/PositionCustomizeDrum";
                break;
            case State.Status.SoundCustomizing:
                drumName = "Prefabs/SoundCustomizeDrum";
                break;
            case State.Status.DesignCustomizing:
                drumName = "Prefabs/DesignCustomizeDrum";
                break;
            default:
                break;
        }

        drum = (GameObject)GameObject.Instantiate(Resources.Load(drumName));
        drum.transform.Translate(neckPos);
        if (nextStatus == State.Status.Playing || nextStatus == State.Status.PositionCustomizing || nextStatus == State.Status.PositionMoving || nextStatus == State.Status.DesignCustomizing)
            customizePosition();
        if (nextStatus == State.Status.DesignCustomizing)
            customizeDesign();
        Camera.SetupCurrent(drum.GetComponentInChildren<Camera>());
    }

    public void customizeDesign()
    {
        State state = GameObject.Find("MAIN").GetComponent<main>()._State;
        Color color = state.SelectedDesign == State.CustomizeDesignOption.Standard ? Color.black : new Color((float)0.973, (float)0.286, (float)0.953, (float)0.97);
        for (int i = 0; i < 10; i++)
        {
            Debug.Log("before : " + GameObject.Find(state.drum_parts[i]).GetComponent<MeshRenderer>().material.GetColor("_EmissionColor"));
            GameObject.Find(state.drum_parts[i]).GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", color);
            Debug.Log("after : " + GameObject.Find(state.drum_parts[i]).GetComponent<MeshRenderer>().material.GetColor("_EmissionColor"));
        }
    }

    public void customizePosition()
    {
        State state = GameObject.Find("MAIN").GetComponent<main>()._State;
        for (int i = 0; i < 7; i++)
        {
            string drumName = state.drum_names[i];
            if(PlayerPrefs.HasKey(drumName + "X"))
            {
                GameObject.Find(drumName).transform.Translate(new Vector3(PlayerPrefs.GetFloat(drumName + "X"), 0, 0));
            }
            if (PlayerPrefs.HasKey(drumName + "Z"))
            {
                GameObject.Find(drumName).transform.Translate(new Vector3(0, 0, PlayerPrefs.GetFloat(drumName + "Z")));
            }
        }
    }

    public void CreateHands()
    {
        leftStick = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/stick"));
        rightStick = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/stick"));
    }

    public void CreatePlayDrum(Kinect.CameraSpacePoint position)
    {
        Vector3 neckPos = Utility.CameraSpacePointToWorld(position);
        drum = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/RealDrum"));
        drum.transform.Translate(neckPos);
        Camera.SetupCurrent(drum.GetComponentInChildren<Camera>());
    }

    public void DestroyPlayDrum()
    {
        GameObject.Destroy(drum);
    }

    public void DestroyMenuDrum()
    {
        GameObject.Destroy(menuDrum);
    }

    public void CreateMenuDrum(Kinect.CameraSpacePoint position)
    {
        Vector3 neckPos = Utility.CameraSpacePointToWorld(position);
        menuDrum = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/MenuDrum"));
        menuDrum.transform.Translate(neckPos);
        Camera.SetupCurrent(drum.GetComponentInChildren<Camera>());
    }

    public void CreateCustomizeMenuDrum(Kinect.CameraSpacePoint position)
    {
        Vector3 neckPos = Utility.CameraSpacePointToWorld(position);
        menuDrum = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/MenuDrum"));
        menuDrum.transform.Translate(neckPos);
        Camera.SetupCurrent(drum.GetComponentInChildren<Camera>());
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
