using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class FootDrum : SoundDrum {
    abstract protected Vector3 kneePos { get; }

    protected void Update()
    {
        base.Update();

        state.updates--;
        state.updates2--;
        if (Hit())
        {
            //Debug.Log(motion.rightKneeVel.y + " : " + this.GetInstanceID() + " : " + state.updates + " , " + state.updates2);
        }
    }

    override protected bool Hit()
    {
        return motion.hitRightFootDrum;
    }
}
