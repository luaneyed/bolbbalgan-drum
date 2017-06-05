using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesignCustomizeConfirmDrum : ButtonDrum
{

    protected override void onHit()
    {
        switch (state.SelectedDesign)
        {
            case State.CustomizeDesignOption.Standard:
                PlayerPrefs.SetInt("DesignIdx", 0);
                break;
            case State.CustomizeDesignOption.BolbbalganDrum:
                PlayerPrefs.SetInt("DesignIdx", 1);
                break;
        }

        GameObject.Find("MAIN").GetComponent<main>().CustomizeMenuMode();
    }

    protected override bool Hit()
    {
        return motion.hitRightFootDrum;
    }
}
