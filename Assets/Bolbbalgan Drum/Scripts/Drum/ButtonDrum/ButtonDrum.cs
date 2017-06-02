using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ButtonDrum : Drum
{
    private bool leftClear = true, rightClear = true;
    private int radius = 2;

    void Start()
    {
        base.Start();
    }

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
            return true;
        }

        return false;
    }
}
