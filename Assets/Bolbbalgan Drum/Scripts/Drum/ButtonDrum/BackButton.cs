using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackButton : ButtonDrum {

    protected void Awake()
    {
        radius = 1;
        leftClear = false;
        rightClear = false;
    }

    protected override void onHit()
    {
        GameObject.Find("MAIN").GetComponent<main>().MenuMode();
    }
}
