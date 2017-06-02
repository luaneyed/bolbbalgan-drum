using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Drum : MonoBehaviour {
    protected MotionAnalyzer motion;
    protected State state;

    protected void Start()
    {
        motion = GameObject.Find("MAIN").GetComponent<main>().Motion;
        state = GameObject.Find("MAIN").GetComponent<main>()._State;
    }

    protected void Update()
    {
        if (state.handsEnabled && Hit())
        {
            onHit();
        }
    }

    abstract protected bool Hit();
    abstract protected void onHit();
}
