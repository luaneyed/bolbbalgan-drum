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
                GameObject.Find("MAIN").GetComponent<main>().PlayMode();
                break;
            case State.MenuOption.Customizing:
                GameObject.Find("MAIN").GetComponent<main>().CustomizeMenuMode();
                state.SelectedCustomizeMenu = State.CustomizeMenuOption.Position;
                break;
            /*case State.CustomizeMenuOption.Position:
                GameObject.Find("MAIN").GetComponent<main>().PositionCustomizeMode();
                break;*/
            case State.MenuOption.Quit:
                Application.Quit();
                break;
        }
    }

    protected override bool Hit()
    {
        return motion.hitRightFootDrum;
    }
}
