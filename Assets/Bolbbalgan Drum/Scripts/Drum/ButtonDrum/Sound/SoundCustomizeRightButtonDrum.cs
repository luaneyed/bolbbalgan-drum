using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundCustomizeRightButtonDrum : ButtonDrum {
    override protected void onHit()
    {
        if (state.SelectedSound == State.CustomizeSoundOption.Standard)
        {
            state.SelectedSound = State.CustomizeSoundOption.HipHop;
        }
        else
        {
            state.SelectedSound = State.CustomizeSoundOption.Standard;
        }
    }
}
