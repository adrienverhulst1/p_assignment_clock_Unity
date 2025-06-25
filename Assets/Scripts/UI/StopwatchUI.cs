using UnityEngine;
using UnityEngine.UI; // or TMPro
using VContainer;
using UniRx;
using System;
using System.Linq;
using Codice.Client.Common;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Play")]
public class StopwatchUI : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text time;
    [SerializeField] private TMPro.TMP_Text laps_1;

    [SerializeField] private Button start_stop_button;
    [SerializeField] private Sprite start_button_img;
    [SerializeField] private Sprite stop_button_img;
    [SerializeField] private Button reset_button;
    [SerializeField] private Button lap_button;

    internal TMPro.TMP_Text TimeText { get { return time; } }
    internal TMPro.TMP_Text LapText { get { return laps_1; } }
    internal Button StartStopButton { get { return start_stop_button; } }
    internal Button ResetButton { get { return reset_button; } }
    internal Button LapButton { get { return lap_button; } }

    private readonly CompositeDisposable composite_disposable = new();

    [Inject]
    public void Construct(IStopwatchService sw)
    {
        sw.ElapsedTime.Subscribe(dt => time.text = dt.ToString(@"mm\:ss\.ffff"))
            .AddTo(composite_disposable);

        start_stop_button.OnClickAsObservable()
            .Subscribe(x => {
                if (sw.CurrentState.Value == SWStatus.Started)
                {
                    sw.Stop.Execute(Unit.Default);
                    start_stop_button.image.sprite = start_button_img;

                    //reset_button.interactable = false;
                    lap_button.interactable = false;
                }
                else if (sw.CurrentState.Value == SWStatus.Default || sw.CurrentState.Value == SWStatus.Paused)
                {
                    sw.Start.Execute(Unit.Default);
                    start_stop_button.image.sprite = stop_button_img;

                    //reset_button.interactable = true;
                    lap_button.interactable = true;
                }
            })
            .AddTo(composite_disposable);

        reset_button.OnClickAsObservable()
            .Subscribe(x => {
                sw.Reset.Execute(Unit.Default);

                start_stop_button.image.sprite = start_button_img;
                //reset_button.interactable = false;
                lap_button.interactable = false;

                laps_1.text = "# 00:00";
            })
            .AddTo(composite_disposable);

        lap_button.OnClickAsObservable()
            .Subscribe(x => {
                sw.Lap.Execute(Unit.Default);
                if(sw.LapTimes.Count >= 0)
                {
                    laps_1.text = sw.LapTimes.LastOrDefault().ToString(@"mm\:ss\.ffff");
                }
            })
            .AddTo(composite_disposable);
    }

    private void OnDestroy()
    {
        composite_disposable.Dispose();
    }
}