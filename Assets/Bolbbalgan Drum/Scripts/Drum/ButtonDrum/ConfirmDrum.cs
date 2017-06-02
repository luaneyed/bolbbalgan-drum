using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmDrum : ButtonDrum
{

    protected override void onHit()
    {

        switch (state.SelectedMenu)
        {
            case State.MenuOption.Play:
                GameObject.Find("MAIN").GetComponent<main>()._DisplayManager.ActivateDrum();
                GameObject.Find("MAIN").GetComponent<main>()._DisplayManager.DeactivateMenuDrum();
                state.MainStatus = State.Status.Playing;
                break;
            case State.MenuOption.Customizing:
                GameObject.Find("MAIN").GetComponent<main>()._DisplayManager.DeactivateMenuDrum();
                break;
        }
    }

    protected override bool Hit()
    {
        return motion.hitRightFootDrum;
    }
}
