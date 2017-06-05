using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizeLeftButtonDrum : ButtonDrum {

    override protected void onHit()
    {
        if (state.SelectedCustomizeMenu != State.CustomizeMenuOption.Position)
        {
            state.SelectedCustomizeMenu--;
        }
        else
        {
            state.SelectedCustomizeMenu = State.CustomizeMenuOption.Back;
        }
    }
}
