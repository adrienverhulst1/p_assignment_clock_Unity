using UnityEngine;
using UnityEngine.UI; // or TMPro
using VContainer;
using UniRx;
using System;

public class TimerUI : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text time;
    [SerializeField] private Button play_button;
    [SerializeField] private Button pause_button;
    [SerializeField] private Button reset_button;

    private readonly CompositeDisposable composite_disposable = new();

    [Inject]
    public void Construct(ITimerService timer_service)
    {
        play_button.OnClickAsObservable()
            .Subscribe(_ => timer_service.Start.Execute(TimeSpan.FromSeconds(10)))
            .AddTo(this);

        pause_button.OnClickAsObservable()
            .Subscribe(_ => timer_service.Pause.Execute(Unit.Default))
            .AddTo(this);

        reset_button.OnClickAsObservable()
            .Subscribe(_ => timer_service.Reset.Execute(Unit.Default))
            .AddTo(this);

        timer_service.RemainingTime.Subscribe(dt => time.text = dt.ToString(@"mm\:ss\.fff"))
            .AddTo(composite_disposable);

        timer_service.CurrentState.Subscribe(x =>
        {
            if(x == TStatus.Stopped)
            {
                play_button.gameObject.SetActive(true);
                pause_button.gameObject.SetActive(false);
            }
            if (x == TStatus.Started)
            {
                play_button.gameObject.SetActive(false);
                pause_button.gameObject.SetActive(true);
            }
            if (x == TStatus.Paused)
            {
                play_button.gameObject.SetActive(true);
                pause_button.gameObject.SetActive(false);
            }
        });
    }

    private void OnDestroy()
    {
        composite_disposable.Dispose();
    }
}