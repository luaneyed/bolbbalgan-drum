using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State {
    public string str = "For Debug";

    public bool handsEnabled = false;
    public int updates = 0;
    public int updates2 = 0;

    public enum Status
    {
        Initial, Playing, Menu
    }
    public Status MainStatus;

    public enum MenuOption
    {
        Play, Customizing
    }
    public MenuOption SelectedMenu;

    public enum DrumPart
    {
        HiHat, Crash, Ride, Snare, HighTom, MidTom, FloorTom
    }
    public DrumPart SelectedDrum;
}
