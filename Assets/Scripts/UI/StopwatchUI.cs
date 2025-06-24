using UnityEngine;
using UnityEngine.UI; // or TMPro
using VContainer;
using UniRx;
using System;
using System.Linq;
using Codice.Client.Common;

public class StopwatchUI : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text time;
    [SerializeField] private TMPro.TMP_Text laps_1;

    [SerializeField] private Button start_stop_button;
    [SerializeField] private Sprite start_button_img;
    [SerializeField] private Sprite stop_button_img;
    [SerializeField] private Button reset_button;
    [SerializeField] private Button lap_button;

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

                    DisableButtonHelper(reset_button);
                    DisableButtonHelper(lap_button);
                }
                else if (sw.CurrentState.Value == SWStatus.Default || sw.CurrentState.Value == SWStatus.Stopped)
                {
                    sw.Start.Execute(Unit.Default);
                    start_stop_button.image.sprite = stop_button_img;

                    EnableButtonHelper(reset_button);
                    EnableButtonHelper(lap_button);
                }
            })
            .AddTo(composite_disposable);

        reset_button.OnClickAsObservable()
            .Subscribe(x => {
                sw.Reset.Execute(Unit.Default);

                start_stop_button.image.sprite = start_button_img;
                DisableButtonHelper(reset_button);
                DisableButtonHelper(lap_button);

                laps_1.text = "#1 00:00";
            })
            .AddTo(composite_disposable);

        lap_button.OnClickAsObservable()
            .Subscribe(x => {
                sw.Lap.Execute(Unit.Default);
                if(sw.LapTimes.Count >= 0)
                {
                    laps_1.text = sw.LapTimes.LastOrDefault().ToString();
                }
            })
            .AddTo(composite_disposable);
    }

    private void OnDestroy()
    {
        composite_disposable.Dispose();
    }

    void DisableButtonHelper(Button button)
    {
        button.interactable = false;
        ColorBlock colors = button.colors;
        colors.normalColor = Color.gray;
        button.colors = colors;
    }

    void EnableButtonHelper(Button button)
    {
        button.interactable = true;
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        button.colors = colors;
    }
}