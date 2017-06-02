using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Kinect = Windows.Kinect;

public class MotionAnalyzer
{
    public Vector3 leftHandPos, rightHandPos;
    public Vector3 prevLeftHandPos, prevRightHandPos;
    public Vector3 prevLeftKneePos, prevRightKneePos;
    public Vector3 leftHandVel, rightHandVel;
    public Vector3 leftKneeVel, rightKneeVel;
    public Vector3 initLeftKneePos = Vector3.zero, initRightKneePos = Vector3.zero;
    public Vector3 leftKneePos, rightKneePos;
    public bool isHiHatOpened = true;
    public bool hitLeftFootDrum = false;
    public bool hitRightFootDrum = false;
    private bool leftFootClear, rightFootClear;
    float prevTime;
    
    
    public MotionAnalyzer () {
        leftHandVel = Vector3.zero;
        rightHandVel = Vector3.zero;

        prevTime = 0;
    }
    

    public void Update(KinectManager manager, Kinect.CameraSpacePoint leftTip, Kinect.CameraSpacePoint rightTip)
    {
        leftHandPos = Utility.CameraSpacePointToWorld(leftTip);
        rightHandPos = Utility.CameraSpacePointToWorld(rightTip);

        leftKneePos = Utility.CameraSpacePointToWorld(manager.JointData[Windows.Kinect.JointType.KneeLeft].Position);
        rightKneePos = Utility.CameraSpacePointToWorld(manager.JointData[Windows.Kinect.JointType.KneeRight].Position);

        if (prevTime == 0)
        {
            prevLeftHandPos = leftHandPos;
            prevRightHandPos = rightHandPos;
            prevLeftKneePos = leftKneePos;
            prevRightKneePos = rightKneePos;
        }
        // vel
        float currTime = Time.time;
        float timeDiff = currTime - prevTime;
        leftHandVel = (leftHandPos - prevLeftHandPos) / timeDiff;
        rightHandVel = (rightHandPos - prevRightHandPos) / timeDiff;
        leftKneeVel = (leftKneePos - prevLeftKneePos) / timeDiff;
        rightKneeVel = (rightKneePos - prevRightKneePos) / timeDiff;
        //Debug.Log(rightKneeVel.y);

        // update prev, lasttime
        prevLeftHandPos = leftHandPos;
        prevRightHandPos = rightHandPos;
        prevLeftKneePos = leftKneePos;
        prevRightKneePos = rightKneePos;

        prevTime = currTime;

        checkFootDrum();
    }

    public void onStartPlaying(KinectManager manager)
    {
        initLeftKneePos = Utility.CameraSpacePointToWorld(manager.JointData[Windows.Kinect.JointType.KneeLeft].Position);
        initRightKneePos = Utility.CameraSpacePointToWorld(manager.JointData[Windows.Kinect.JointType.KneeRight].Position);
    }

    void checkFootDrum()
    {
        //Debug.Log(rightKneePos.y - initRightKneePos.y);
        if (initLeftKneePos == Vector3.zero && initRightKneePos == Vector3.zero)
        {
            return;
        }
        if (rightKneePos.y > initRightKneePos.y + 0.3)
        {
            rightFootClear = true;
        }
        else
        {
            if (rightFootClear)
            {
                rightFootClear = false;
                hitRightFootDrum = true;
            }
            else
            {
                hitRightFootDrum = false;
            }
        }
        
        isHiHatOpened = (leftKneePos.y > initLeftKneePos.y + 0.3);
    }
}
