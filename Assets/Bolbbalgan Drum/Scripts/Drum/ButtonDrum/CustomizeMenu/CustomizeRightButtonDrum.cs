using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizeRightButtonDrum : ButtonDrum {
    override protected void onHit()
    {
        if(state.SelectedCustomizeMenu != State.CustomizeMenuOption.Back)
        {
            state.SelectedCustomizeMenu++;
        } else
        {
            state.SelectedCustomizeMenu = State.CustomizeMenuOption.Position;
        }
    }
}
