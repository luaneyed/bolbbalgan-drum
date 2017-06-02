using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract class HandDrum : SoundDrum {
    private bool leftClear = true, rightClear = true;
    private bool byLeft;

    abstract protected float radius { get; }

    override protected bool Hit()
    {
        if (motion.leftHandPos.y > this.transform.position.y)
        {
            leftClear = true;
        }
        if (leftClear && motion.leftHandVel.y < -10 &&
            motion.leftHandPos.y < this.transform.position.y &&
            Utility.XZdistance(motion.leftHandPos, this.transform.position) < radius)
        {
            leftClear = false;
            byLeft = true;
            return true;
        }

        if (motion.rightHandPos.y > this.transform.position.y)
        {
            rightClear = true;
        }
        if (rightClear && motion.rightHandVel.y < -10 &&
            motion.rightHandPos.y < this.transform.position.y &&
            Utility.XZdistance(motion.rightHandPos, this.transform.position) < radius)
        {
            rightClear = false;
            byLeft = false;
            return true;
        }

        return false;
    }

    protected override float getIntensity()
    {
        Vector3 velocity = byLeft ? motion.leftHandVel : motion.rightHandVel;
        if (velocity.y >= -10)
        {
            Debug.LogError("getIntensity() error");
            return -1;
        }
        return Mathf.Min(1, Mathf.Max(0, Mathf.Pow((velocity.y) / (-100), (float)1.5)));
    }
}