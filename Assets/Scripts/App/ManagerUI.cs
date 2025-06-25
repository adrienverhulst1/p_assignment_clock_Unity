using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManagerUI : MonoBehaviour
{
    [SerializeField]
    ClockUI clock_ui;
    [SerializeField]
    TimerUI timer_ui;
    [SerializeField]
    StopwatchUI stopwatch_ui;
    [SerializeField]
    PanelNavigationUI panel_navigation_ui;

    [SerializeField]
    Button exit_button;

    // Start is called before the first frame update
    void Start()
    {
        exit_button.onClick.AddListener(OnQuitButton);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnQuitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
}
