using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesignCustomizeLeftButtonDrum : ButtonDrum {

    override protected void onHit()
    {
        if (state.SelectedDesign == State.CustomizeDesignOption.Standard)
        {
            state.SelectedDesign = State.CustomizeDesignOption.BolbbalganDrum;
            changeColor(1);
        }
        else
        {
            state.SelectedDesign = State.CustomizeDesignOption.Standard;
            changeColor(0);
        }
    }

    void changeColor(int idx)
    {
        Color color = idx == 0 ? Color.black : new Color((float)0.973, (float)0.286, (float)0.953, (float)0.97);
        for (int i = 0; i < 10; i++)
        {
            GameObject.Find(state.drum_parts[i]).GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", color);
        }
    }
}