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

    void Awake ()
    {
        _State = new State();
        _State.MainStatus = State.Status.Initial;
        _KinectManager = new KinectManager();
        _StickRecognizer = new StickRecognizer(_KinectManager);
        _DisplayManager = new DisplayManager(MainCamera);
        Motion = new MotionAnalyzer();

    }
	
	void Update ()
    {
        KinectManager.Status status = _KinectManager.Update();

        _State.updates2++;
        switch (status)
        {
            case KinectManager.Status.ZeroBody:
                Debug.Log("ZeroBody?");
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

            switch(_State.MainStatus)
            {
                case State.Status.Initial:
                    onStartPlaying();
                    _State.MainStatus = State.Status.Menu;
                    _DisplayManager.InitDisplay(_KinectManager);
                    break;
                case State.Status.Playing:
                    break;
                case State.Status.Menu:
                    // Menu에서 Plying은 ButtonDrum Trigger를 통해 넘어가야 함
                    //MainStatus = Status.Playing;
                    //_DisplayManager.ActivateDrum();
                    break;

            }

            // 플레이어는 항상 display 돼야 함
            _DisplayManager.DisplayPlayer(_KinectManager, leftTip, rightTip);
            Motion.Update(_KinectManager, leftTip, rightTip);
            Debug.Log("enabled");
            _State.handsEnabled = true;
        } else
        {
            Debug.Log("disabled");
            _State.handsEnabled = false;
        }
    }

    void onStartPlaying()
    {
        Motion.onStartPlaying(_KinectManager);
    }
}
