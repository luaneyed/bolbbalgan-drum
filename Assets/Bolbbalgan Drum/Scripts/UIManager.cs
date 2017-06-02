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
            gui.fontSize = 20;
        }
        switch (state.MainStatus)
        {
            case State.Status.Initial:
                GUI.Box(new Rect((Screen.width- msgWidth)/2, (Screen.height-msgHeight)/2, msgWidth, msgHeight), "스틱을 들고 키넥트 앞에 앉아주세요.", gui);
                break;
            case State.Status.Playing:
                break;
            case State.Status.Menu:
                string message = (state.SelectedMenu == State.MenuOption.Play) ? "시작" : "커스터마이징";
                GUI.Box(new Rect((Screen.width - msgWidth) / 2, (Screen.height - msgHeight) / 2, msgWidth, msgHeight), message, gui);
                break;
        }
    }
}