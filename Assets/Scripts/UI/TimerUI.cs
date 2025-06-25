using UnityEngine;
using UnityEngine.UI; // or TMPro
using VContainer;
using UniRx;
using System;
using System.Runtime.CompilerServices;
using TMPro;

[assembly: InternalsVisibleTo("Play")]
public class TimerUI : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text time;
    [SerializeField] private Button play_button;
    [SerializeField] private Button pause_button;
    [SerializeField] private Button reset_button;
    [SerializeField] private TMPro.TMP_InputField input_field;
    [SerializeField] private AudioSource audio_source;
    [SerializeField] private AudioClip on_finished_audio_clip;

    internal TMPro.TMP_Text TimeText { get { return time; } }
    internal Button PlayButton { get { return play_button; } }
    internal Button PauseButton { get { return pause_button; } }
    internal Button ResetButton { get { return reset_button; } }
    internal TMPro.TMP_InputField InputField { get { return input_field; } }

    private readonly CompositeDisposable composite_disposable = new();

    [Inject]
    public void Construct(ITimerService timer_service)
    {
        //input_field.onValueChanged.AsObservable()
        //    .Subscribe(x => timer_service.SetTargetTime.Execute(x))
        //    .AddTo(composite_disposable);
        input_field.onEndEdit.AsObservable()
            .Subscribe(x => timer_service.SetTargetTime.Execute(x))
            .AddTo(composite_disposable);

        play_button.OnClickAsObservable()
            .Subscribe(_ => timer_service.Start.Execute(Unit.Default))
            .AddTo(composite_disposable);

        pause_button.OnClickAsObservable()
            .Subscribe(_ => timer_service.Pause.Execute(Unit.Default))
            .AddTo(composite_disposable);

        reset_button.OnClickAsObservable()
            .Subscribe(_ => timer_service.Reset.Execute(Unit.Default))
            .AddTo(composite_disposable);

        timer_service.OnFinished.Subscribe(x => { audio_source.PlayOneShot(on_finished_audio_clip); });

        timer_service.RemainingTime.Subscribe(dt => input_field.text = dt.ToString(@"mm\:ss"))
            .AddTo(composite_disposable);

        timer_service.CurrentState.Subscribe(x =>
        {
            if(x == TStatus.Stopped)
            {
                play_button.interactable = true;
                pause_button.interactable = false;
            }
            if (x == TStatus.Started)
            {
                play_button.interactable = false;
                pause_button.interactable = true;
            }
            if (x == TStatus.Paused)
            {
                play_button.interactable = true;
                pause_button.interactable = false;
            }
        });
    }

    private void OnDestroy()
    {
        composite_disposable.Dispose();
    }
}