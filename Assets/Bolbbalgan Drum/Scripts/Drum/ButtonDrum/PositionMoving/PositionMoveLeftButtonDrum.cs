using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionMoveLeftButtonDrum : ButtonDrum {

    override protected void onHit()
    {
        string currDrum = state.drum_names[((int)state.SelectedDrum)];
        if (PlayerPrefs.HasKey(currDrum + "X"))
        {
            float currentPos = PlayerPrefs.GetFloat(currDrum + "X");
            PlayerPrefs.SetFloat(currDrum + "X", currentPos - (float)0.05);
            GameObject.Find(currDrum).transform.Translate(new Vector3((float)-0.05, 0, 0));
        }
        else
        {
            PlayerPrefs.SetFloat(currDrum + "X", (float)-0.05);
            GameObject.Find(currDrum).transform.Translate(new Vector3((float)-0.05, 0, 0));
        }
    }
}
