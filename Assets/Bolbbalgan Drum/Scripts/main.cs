using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Kinect = Windows.Kinect;

public class main : MonoBehaviour {
    public Camera MainCamera;

    public MotionAnalyzer Motion { get; private set; }

    private KinectManager _KinectManager;
    private StickRecognizer _StickRecognizer;
    public DisplayManager _DisplayManager;
    public State _State;

    int counter = 10;

    void Start()
    {
        _State = new State();
        _State.MainStatus = State.Status.Initial;

        if (!PlayerPrefs.HasKey("SoundIdx"))
        {
            PlayerPrefs.SetInt("SoundIdx", 0);
        }
        if (!PlayerPrefs.HasKey("DesignIdx"))
        {
            PlayerPrefs.SetInt("DesignIdx", 0);
        }

        _State.SelectedSound = (State.CustomizeSoundOption) PlayerPrefs.GetInt("SoundIdx");
        _State.SelectedDesign = (State.CustomizeDesignOption)PlayerPrefs.GetInt("DesignIdx");
    }

    void Awake()
    {
        _KinectManager = new KinectManager();
        _StickRecognizer = new StickRecognizer(_KinectManager);
        _DisplayManager = new DisplayManager(MainCamera);
        Motion = new MotionAnalyzer();
    }

    void Update()
    {
        KinectManager.Status status = _KinectManager.Update();

        _State.updates2++;
        switch (status)
        {
            case KinectManager.Status.ZeroBody:
                //Debug.Log("ZeroBody?");
                _State.handsEnabled = false;
                _State.updates++;
                return;

            case KinectManager.Status.MultiBody:
            case KinectManager.Status.OneBody:
                break;
        }
        _State.updates++;

        bool leftStatus, rightStatus;
        Kinect.CameraSpacePoint leftTip, rightTip;
        _StickRecognizer.FindTip(_KinectManager, out leftTip, out rightTip, out leftStatus, out rightStatus);
        if (leftStatus && rightStatus)
        {
            counter--;
            if (counter > 0)
                return;

            switch (_State.MainStatus)
            {
                case State.Status.Initial:
                    Motion.onStartPlaying(_KinectManager);
                    _DisplayManager.CreateHands();
                    _State.MainStatus = State.Status.Menu;
                    _DisplayManager.ChangeDrum(_State.MainStatus, _KinectManager.JointData[Kinect.JointType.Neck].Position);
                    break;
                default:
                    break;
            }

            // 플레이어는 항상 display 돼야 함
            _DisplayManager.DisplayPlayer(_KinectManager, leftTip, rightTip);
            Motion.Update(_KinectManager, leftTip, rightTip);
            _State.handsEnabled = true;
        } else
        {
            _State.handsEnabled = false;
        }
    }

    public void PlayMode()
    {
        _State.MainStatus = State.Status.Playing;
        _DisplayManager.ChangeDrum(_State.MainStatus, _KinectManager.JointData[Kinect.JointType.Neck].Position);
    }

    public void MenuMode()
    {
        _State.MainStatus = State.Status.Menu;
        _DisplayManager.ChangeDrum(_State.MainStatus, _KinectManager.JointData[Kinect.JointType.Neck].Position);
    }

    public void CustomizeMenuMode()
    {
        _State.MainStatus = State.Status.CustomizeMenu;
        _DisplayManager.ChangeDrum(_State.MainStatus, _KinectManager.JointData[Kinect.JointType.Neck].Position);
    }

    public void PositionCustomizeMenuMode()
    {
        _State.MainStatus = State.Status.PositionCustomizing;
        _DisplayManager.ChangeDrum(_State.MainStatus, _KinectManager.JointData[Kinect.JointType.Neck].Position);
    }
    public void PositionCustomizeMode()
    {
        _State.MainStatus = State.Status.PositionMoving;
        _DisplayManager.ChangeDrum(_State.MainStatus, _KinectManager.JointData[Kinect.JointType.Neck].Position);
    }

    public void SoundCustomizeMode()
    {
        _State.MainStatus = State.Status.SoundCustomizing;
        _DisplayManager.ChangeDrum(_State.MainStatus, _KinectManager.JointData[Kinect.JointType.Neck].Position);
    }

    public void DesignCustomizeMode()
    {
        _State.MainStatus = State.Status.DesignCustomizing;
        _DisplayManager.ChangeDrum(_State.MainStatus, _KinectManager.JointData[Kinect.JointType.Neck].Position);

    }
}
