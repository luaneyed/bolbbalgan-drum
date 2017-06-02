using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightButtonDrum : ButtonDrum {
    override protected void onHit()
    {
        state.SelectedMenu = State.MenuOption.Customizing;
    }
}
