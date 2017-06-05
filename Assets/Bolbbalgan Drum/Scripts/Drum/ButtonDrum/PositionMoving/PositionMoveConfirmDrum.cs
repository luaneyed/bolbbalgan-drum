using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionMoveConfirmDrum : ButtonDrum
{

    protected override void onHit()
    {
        GameObject.Find("MAIN").GetComponent<main>().CustomizeMenuMode();
    }

    protected override bool Hit()
    {
        return motion.hitRightFootDrum;
    }
}
