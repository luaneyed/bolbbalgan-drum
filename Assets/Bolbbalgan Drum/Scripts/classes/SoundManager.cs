using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Kinect = Windows.Kinect;

public class SoundManager
{
    public AudioSource hithat;
    public AudioSource snare;

    public void Play(KinectManager manager, Kinect.CameraSpacePoint left, Kinect.CameraSpacePoint right)
    {
    }
}
