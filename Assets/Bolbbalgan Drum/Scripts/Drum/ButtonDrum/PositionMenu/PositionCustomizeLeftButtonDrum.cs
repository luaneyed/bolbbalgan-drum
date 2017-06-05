using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionCustomizeLeftButtonDrum : ButtonDrum {

    override protected void onHit()
    {
        if (state.SelectedDrum != State.DrumPart.HiHat)
        {
            GameObject.Find(state.drum_names[((int)state.SelectedDrum - 1)]).GetComponent<MeshRenderer>().material.color = Color.green;
            GameObject.Find(state.drum_names[((int)state.SelectedDrum)]).GetComponent<MeshRenderer>().material.color = Color.white;
            state.SelectedDrum --;
        }
        else
        {
            GameObject.Find("Floor_tom").GetComponent<MeshRenderer>().material.color = Color.green;
            GameObject.Find("Hi-hat").GetComponent<MeshRenderer>().material.color = Color.white;
            state.SelectedDrum = State.DrumPart.FloorTom;
        }
    }
}
