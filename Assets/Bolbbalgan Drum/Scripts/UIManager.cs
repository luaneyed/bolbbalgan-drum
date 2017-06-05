using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {
    protected State state;
    public main context;
    private const int msgWidth = 500, msgHeight= 100;
    GUIStyle gui = null;

    void Start()
    {
        state = context._State;
    }

    void OnGUI()
    {
        if (gui == null)
        {
            gui = new GUIStyle(GUI.skin.box);
            gui.alignment = TextAnchor.MiddleCenter;
            gui.fontSize = 30;
            gui.font = (Font) Resources.Load("Fonts/HOONJUNGLEBOOK", typeof(Font));
        }
        switch (state.MainStatus)
        {
            case State.Status.Initial:
                GUI.Box(new Rect((Screen.width- msgWidth)/2, (Screen.height-msgHeight)/2, msgWidth, msgHeight), "스틱을 들고 키넥트 앞에 앉아주세요.", gui);
                break;
            case State.Status.Playing:
                break;
            case State.Status.Menu:
                string message = "";
                switch (state.SelectedMenu)
                {
                    case State.MenuOption.Play:
                        message = "시작";
                        break;
                    case State.MenuOption.Customizing:
                        message = "커스터마이징";
                        break;
                    case State.MenuOption.Quit:
                        message = "종료";
                        break;
                }
                GUI.Box(new Rect((Screen.width - msgWidth) / 2, (Screen.height - msgHeight) / 2, msgWidth, msgHeight), message, gui);
                break;
            case State.Status.CustomizeMenu:
                GUI.Box(new Rect((Screen.width - msgWidth) / 2, (Screen.height - msgHeight) / 2, msgWidth, msgHeight), state.SelectedCustomizeMenu.ToString(), gui);
                break;
            case State.Status.PositionCustomizing:
                GUI.Box(new Rect((Screen.width - msgWidth) / 2, (Screen.height - msgHeight) / 2, msgWidth, msgHeight), state.SelectedDrum.ToString(), gui);
                break;
            case State.Status.SoundCustomizing:
                GUI.Box(new Rect((Screen.width - msgWidth) / 2, (Screen.height - msgHeight) / 2, msgWidth, msgHeight), state.SelectedSound.ToString(), gui);
                break;
            case State.Status.DesignCustomizing:
                GUI.Box(new Rect((Screen.width - msgWidth) / 2, (Screen.height - msgHeight) / 2, msgWidth, msgHeight), state.SelectedDesign.ToString(), gui);
                break;
        }
    }
}