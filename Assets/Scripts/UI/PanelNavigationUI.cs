using UnityEngine;
using UnityEngine.UI; // or TMPro
using VContainer;
using UniRx;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Play")]
public class PanelNavigationUI : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text title;
    [SerializeField] private GameObject timer_panel;
    [SerializeField] private Button timer_button;
    [SerializeField] private GameObject timer_highlight;
    [SerializeField] private GameObject stopwatch_panel;
    [SerializeField] private Button stopwatch_button;
    [SerializeField] private GameObject stopwatch_highlight;

    internal TMPro.TMP_Text Title { get { return title; } }
    internal GameObject TimerPanel { get { return timer_panel; } }
    internal Button TimerButton { get { return timer_button; } }
    internal GameObject StopwatchPanel { get { return stopwatch_panel; } }
    internal Button StopwatchButton { get { return stopwatch_button; } }

    private readonly CompositeDisposable composite_disposable = new();

    [Inject]
    public void Construct()
    {
        timer_button.OnClickAsObservable().Subscribe(x => {
            ActivateTimerPanel();
        })
            .AddTo(composite_disposable);

        stopwatch_button.OnClickAsObservable().Subscribe(x => {
            ActivateStopwatchPanel();
        })
            .AddTo(composite_disposable);

        ActivateTimerPanel();
    }

    internal void ActivateTimerPanel()
    {
        title.text = "Timer";
        timer_panel.SetActive(true);
        stopwatch_panel.SetActive(false);
        //timer_button.interactable = false;
        //stopwatch_button.interactable = true;
        timer_highlight.SetActive(true);
        stopwatch_highlight.SetActive(false);
    }

    internal void ActivateStopwatchPanel()
    {
        title.text = "Stopwatch";
        timer_panel.SetActive(false);
        stopwatch_panel.SetActive(true);
        //timer_button.interactable = true;
        //stopwatch_button.interactable = false;
        timer_highlight.SetActive(false);
        stopwatch_highlight.SetActive(true);
    }

    private void OnDestroy()
    {
        composite_disposable.Dispose();
    }
}