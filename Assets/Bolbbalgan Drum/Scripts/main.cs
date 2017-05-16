using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Kinect = Windows.Kinect;

public class main : MonoBehaviour {
    private KinectManager _KinectManager;
    private StickRecognizer _StickRecognizer;
    private DisplayManager _DisplayManager;

    void Start () {
        _KinectManager = new KinectManager();
        _StickRecognizer = new StickRecognizer(_KinectManager);
        _DisplayManager = new DisplayManager();
    }
	
	void Update () {
        KinectManager.Status status = _KinectManager.Update();
        
        switch(status)
        {
            case KinectManager.Status.ZeroBody:
                return;

            case KinectManager.Status.MultiBody:
            case KinectManager.Status.OneBody:
                break;
        }

        bool leftStatus, rightStatus;
        Kinect.CameraSpacePoint leftTip, rightTip;
        _StickRecognizer.FindTip(_KinectManager, out leftTip, out rightTip, out leftStatus, out rightStatus);

        if(leftStatus && rightStatus)
        {
            //_DisplayManager.DrawBody(_KinectManager);
            _DisplayManager.DisplayPlayer(_KinectManager, leftTip, rightTip);
        }
        // MotionAnalyser
        // SoundManger
        // DisplayManager
    }
}
