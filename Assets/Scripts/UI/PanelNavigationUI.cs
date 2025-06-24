using UnityEngine;
using UnityEngine.UI; // or TMPro
using VContainer;
using UniRx;
using System;
using System.Linq;

public class PanelNavigationUI : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text title;
    [SerializeField] private GameObject timer_panel;
    [SerializeField] private Button timer_button;
    [SerializeField] private GameObject stopwatch_panel;
    [SerializeField] private Button stopwatch_button;

    private readonly CompositeDisposable composite_disposable = new();

    [Inject]
    public void Construct()
    {
        timer_button.OnClickAsObservable().Subscribe(x => {
            title.text = "Timer";
            timer_panel.SetActive(true);
            stopwatch_panel.SetActive(false);

            Debug.Log("hi");
        })
            .AddTo(composite_disposable);

        stopwatch_button.OnClickAsObservable().Subscribe(x => {
            title.text = "Stopwatch";
            timer_panel.SetActive(false);
            stopwatch_panel.SetActive(true);
        })
            .AddTo(composite_disposable);
    }

    private void OnDestroy()
    {
        composite_disposable.Dispose();
    }
}