using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class KickDrum : FootDrum {
    protected override Vector3 kneePos { get {return motion.rightKneePos;} }

    protected override float getIntensity()
    {
        return Mathf.Min(1, Mathf.Max(0, (motion.rightKneeVel.y) / (-10)));
    }
}
