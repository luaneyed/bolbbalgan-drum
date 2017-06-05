using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizeConfirmDrum : ButtonDrum
{

    protected override void onHit()
    {
        switch (state.SelectedCustomizeMenu)
        {
            case State.CustomizeMenuOption.Position:
                GameObject.Find("MAIN").GetComponent<main>().PositionCustomizeMenuMode();
                GameObject.Find("Hi-hat").GetComponent<MeshRenderer>().material.color = Color.green;
                state.SelectedDrum = State.DrumPart.HiHat;
                break;
            case State.CustomizeMenuOption.Sound:
                GameObject.Find("MAIN").GetComponent<main>().SoundCustomizeMode();
                break;
            case State.CustomizeMenuOption.Design:
                state.SelectedDesign = (State.CustomizeDesignOption)PlayerPrefs.GetInt("DesignIdx");
                GameObject.Find("MAIN").GetComponent<main>().DesignCustomizeMode();
                break;
            case State.CustomizeMenuOption.Back:
                GameObject.Find("MAIN").GetComponent<main>().MenuMode();
                break;
        }
    }

    protected override bool Hit()
    {
        return motion.hitRightFootDrum;
    }
}
