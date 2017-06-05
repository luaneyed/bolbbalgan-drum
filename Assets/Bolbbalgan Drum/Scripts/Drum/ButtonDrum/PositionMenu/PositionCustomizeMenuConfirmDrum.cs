using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionCustomizeMenuConfirmDrum : ButtonDrum
{

    protected override void onHit()
    {
        GameObject.Find("MAIN").GetComponent<main>().PositionCustomizeMode();
        GameObject.Find(state.drum_names[((int)state.SelectedDrum)]).GetComponent<MeshRenderer>().material.color = Color.green;
    }

    protected override bool Hit()
    {
        return motion.hitRightFootDrum;
    }
}
