using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State {
    public string str = "For Debug";
    public string[] drum_names = new string[] { "Hi-hat", "Crash", "Ride", "Snare_drum", "High_tom", "Middle_tom", "Floor_tom" };
    public string[] drum_parts = new string[] { "Hi-hat", "Crash", "Ride", "Snare_drum", "High_tom", "Middle_tom", "Floor_tom", "Bass_drum", "Pedal", "Stend" };

    public bool handsEnabled = false;
    public int updates = 0;
    public int updates2 = 0;

    public enum Status
    {
        Initial, Playing, Menu, CustomizeMenu, PositionCustomizing, PositionMoving, SoundCustomizing, DesignCustomizing
    }
    public Status MainStatus;

    public enum MenuOption
    {
        Play, Customizing, Quit
    }

    public MenuOption SelectedMenu;

    public enum CustomizeMenuOption
    {
        Position, Sound, Design, Back
    }

    public CustomizeMenuOption SelectedCustomizeMenu;

    public enum CustomizeSoundOption
    {
        Standard, HipHop
    }

    public CustomizeSoundOption SelectedSound;

    public enum DrumPart
    {
        HiHat, Crash, Ride, Snare, HighTom, MidTom, FloorTom
    }
    public DrumPart SelectedDrum;

    public enum CustomizeDesignOption
    {
        Standard, BolbbalganDrum
    }
    public CustomizeDesignOption SelectedDesign;
}
