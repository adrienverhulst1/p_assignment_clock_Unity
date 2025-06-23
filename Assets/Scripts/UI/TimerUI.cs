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
            .Subscribe(_ => timer_service.Start(TimeSpan.FromSeconds(10)))
            .AddTo(this);

        pause_button.OnClickAsObservable()
            .Subscribe(_ => timer_service.Pause())
            .AddTo(this);

        reset_button.OnClickAsObservable()
            .Subscribe(_ => timer_service.Reset())
            .AddTo(this);

        timer_service.RemainingTime.Subscribe(dt => time.text = dt.ToString(@"mm\:ss\.fff"))
            .AddTo(composite_disposable);

        timer_service.CurrentState.Subscribe(x =>
        {
            if(x == 0)
            {
                play_button.gameObject.SetActive(true);
                pause_button.gameObject.SetActive(false);
            }
            if (x == 1)
            {
                play_button.gameObject.SetActive(false);
                pause_button.gameObject.SetActive(true);
            }
            if (x == 2)
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