using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Kinect = Windows.Kinect;


public class KinectManager {
    public enum Status
    {
        ZeroBody, OneBody, MultiBody
    }

    private Kinect.KinectSensor _Sensor;
    private Kinect.MultiSourceFrameReader _Reader;

    /* Private data from Kinect */
    private Kinect.Body[] _BodyData;

    /* Public data from Kinect */
    public ushort[] DepthData { get; private set; }
    public Dictionary<Kinect.JointType, Kinect.Joint> JointData { get; private set; }

    /* Data schema */
    public Kinect.FrameDescription DepthFrameDesc{ get; private set; }
    public Kinect.CoordinateMapper Mapper { get; private set; }

    public KinectManager()
    {
        _Sensor = Kinect.KinectSensor.GetDefault();
        if(_Sensor == null) ExitWithLog("Kinect Sensor not avalibalbe");

        _Reader = _Sensor.OpenMultiSourceFrameReader(
            Kinect.FrameSourceTypes.Depth |
            Kinect.FrameSourceTypes.Body
        );
        if (_Reader == null) ExitWithLog("Fail to load multiframe source reader.");

        DepthFrameDesc = _Sensor.DepthFrameSource.FrameDescription;
        Mapper = _Sensor.CoordinateMapper;

        DepthData = new ushort[DepthFrameDesc.LengthInPixels];
        _BodyData = new Kinect.Body[_Sensor.BodyFrameSource.BodyCount];

        if(!_Sensor.IsOpen)
        {
            _Sensor.Open();
        }
    }

    public Status Update()
    {
        Kinect.MultiSourceFrame frame = _Reader.AcquireLatestFrame();
        if(frame == null)
        {
            Debug.LogWarning("Frame not arrived");
            return Status.ZeroBody;
        }

        Kinect.DepthFrame depthFrame = frame.DepthFrameReference.AcquireFrame();

        if(depthFrame == null) { return Status.ZeroBody; }

        depthFrame.CopyFrameDataToArray(DepthData);
        depthFrame.Dispose();
        depthFrame = null;

        Kinect.BodyFrame bodyFrame = frame.BodyFrameReference.AcquireFrame();
        
        if (bodyFrame == null) { return Status.ZeroBody; }
        
        bodyFrame.GetAndRefreshBodyData(_BodyData);
        bodyFrame.Dispose();
        bodyFrame = null;

        int bodyIdx;
        Status status;
        checkTrackedBody(out bodyIdx, out status);
        if(status!=Status.ZeroBody)
            JointData = _BodyData[bodyIdx].Joints;

        return status;
    }

    private void checkTrackedBody(out int mainBodyIdx, out Status status)
    {
        // TODO : find closest body?
        int bodyCounter = 0;
        mainBodyIdx = -1;

        for(int i = 0; i < _Sensor.BodyFrameSource.BodyCount; i++)
        {
            if(_BodyData[i].IsTracked)
            {
                mainBodyIdx = i;
                bodyCounter++;
            }
        }

        if(bodyCounter > 1)
        {
            status = Status.MultiBody;
        } else if(bodyCounter == 1)
        {
            status = Status.OneBody;
        } else
        {
            status = Status.ZeroBody;
        }
    }

    static public void ExitWithLog(string log)
    {
        Debug.LogError(log);
        Debug.LogError(UnityEngine.StackTraceUtility.ExtractStackTrace());
        Application.Quit();
    }
}
