using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundCustomizeConfirmDrum : ButtonDrum
{

    protected override void onHit()
    {
        
        switch (state.SelectedSound)
        {
            case State.CustomizeSoundOption.Standard:
                PlayerPrefs.SetInt("SoundIdx", 0);
                break;
            case State.CustomizeSoundOption.HipHop:
                PlayerPrefs.SetInt("SoundIdx", 1);
                break;
        }

        GameObject.Find("MAIN").GetComponent<main>().CustomizeMenuMode();
    }

    protected override bool Hit()
    {
        return motion.hitRightFootDrum;
    }
}
