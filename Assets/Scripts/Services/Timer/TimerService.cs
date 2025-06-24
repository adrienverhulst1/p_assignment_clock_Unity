using System;
using System.Diagnostics;
using TMPro;
using UniRx;
using static UnityEngine.GraphicsBuffer;

public class TimerService : ITimerService, IDisposable
{
    public IReadOnlyReactiveProperty<TStatus> CurrentState => current_state;
    private readonly ReactiveProperty<TStatus> current_state = new ReactiveProperty<TStatus>(TStatus.Stopped);
    public IReadOnlyReactiveProperty<TimeSpan> RemainingTime => remaining_time;
    readonly ReactiveProperty<TimeSpan> remaining_time = new(TimeSpan.Zero);

    public IObservable<Unit> OnFinished => on_finished.AsObservable();
    readonly Subject<Unit> on_finished = new();

    readonly CompositeDisposable composite_disposable = new();

    ITimeInternal time_internal;
    TimeSpan time_elapsed_at_pause;
    TimeSpan time_offset;
    TimeSpan time_elapsed_at_beginning;
    TimeSpan target_time;

    public IReactiveCommand<TimeSpan> Start { get; }
    public IReactiveCommand<Unit> Pause { get; }
    public IReactiveCommand<Unit> Resume { get; }
    public IReactiveCommand<Unit> Reset { get; }

    public TimerService(ITimeInternal time_internal)
    {
        this.time_internal = time_internal;

        time_internal.Now_Elapsed_Time
            .WithLatestFrom(CurrentState, (now, state) => new { now, state })
            .Where(x => x.state == TStatus.Started
            || x.state == TStatus.Paused)
            .Subscribe(x => {
                if (x.state == TStatus.Started) remaining_time.Value = target_time - (x.now - time_elapsed_at_beginning - time_offset);
                else time_offset = x.now - time_elapsed_at_pause;
                if (remaining_time.Value <= TimeSpan.Zero)
                {
                    current_state.Value = 0;
                    remaining_time.Value = TimeSpan.Zero;
                    on_finished.OnNext(Unit.Default);
                    //on_finished.OnCompleted();
                }
            })
            .AddTo(composite_disposable);

        Start = new ReactiveCommand<TimeSpan>(CurrentState
            .Select(x => x == TStatus.Stopped));
        Start
            .WithLatestFrom(CurrentState, (now, state) => new { now, state })
            .Where(x => x.state == TStatus.Stopped)
            .Subscribe(x => {
                if (x.now < TimeSpan.Zero) throw new ArgumentOutOfRangeException();
                target_time = x.now;

                remaining_time.Value = target_time;
                time_elapsed_at_beginning = time_internal.Now_Elapsed_Time.Value;
                current_state.Value = TStatus.Started;
            })
            .AddTo(composite_disposable);

        Pause = new ReactiveCommand<Unit>(CurrentState
            .Select(x => x == TStatus.Started));
        Pause
            .WithLatestFrom(CurrentState, (now, state) => new { now, state })
            .WithLatestFrom(time_internal.Now_Elapsed_Time, (x, target) => new {
                now = x.now,
                state = x.state,
                target = target
            })
            .Where(x => x.state == TStatus.Started)
        .Subscribe(x => {
                time_elapsed_at_pause = x.target;
                current_state.Value = TStatus.Paused; 
            })
            .AddTo(composite_disposable);

        Resume = new ReactiveCommand<Unit>(CurrentState
            .Select(x => x == TStatus.Paused));
        Resume
            .WithLatestFrom(CurrentState, (now, state) => new { now, state })
            .Where(x => x.state == TStatus.Paused)
            .Subscribe(x => {
                current_state.Value = TStatus.Started; 
            })
            .AddTo(composite_disposable);

        Reset = new ReactiveCommand<Unit>(CurrentState
            .Select(x => x == TStatus.Started));
        Reset.WithLatestFrom(CurrentState, (now, state) => new { now, state })
            .Where(x => x.state == TStatus.Started)
            .Subscribe(x => {
                remaining_time.Value = TimeSpan.Zero;
                current_state.Value = TStatus.Stopped; 
            })
            .AddTo(composite_disposable);
    }

    public void Dispose()
    {
        composite_disposable.Dispose();
    }
}