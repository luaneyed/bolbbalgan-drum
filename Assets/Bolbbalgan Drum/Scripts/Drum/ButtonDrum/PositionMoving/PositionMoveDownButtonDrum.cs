using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionMoveDownButtonDrum : ButtonDrum {
    override protected void onHit()
    {
        string currDrum = state.drum_names[((int)state.SelectedDrum)];
        if (PlayerPrefs.HasKey(currDrum + "Z"))
        {
            float currentPos = PlayerPrefs.GetFloat(currDrum + "Z");
            PlayerPrefs.SetFloat(currDrum + "Z", currentPos + (float)0.05);
            GameObject.Find(currDrum).transform.Translate(new Vector3(0, 0, (float)0.05));
        }
        else
        {
            PlayerPrefs.SetFloat(currDrum + "Z", (float)0.05);
            GameObject.Find(currDrum).transform.Translate(new Vector3(0, 0, (float)0.05));
        }
        
    }
}
