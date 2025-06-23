using System;
using System.Diagnostics;
using UniRx;

public class TimerService : ITimerService, IDisposable
{
    public IReadOnlyReactiveProperty<TimeSpan> RemainingTime => remaining_time;
    public IObservable<Unit> OnFinished => on_finished.AsObservable();

    readonly ReactiveProperty<TimeSpan> remaining_time = new(TimeSpan.Zero);
    readonly Subject<Unit> on_finished = new();
    readonly CompositeDisposable composite_disposable = new();
    readonly TimeInternal time_internal;

    TimeSpan time_elapsed_at_begin;

    public TimerService(TimeInternal time_internal)
    {
        this.time_internal = time_internal;
    }

    public void Start(TimeSpan duration)
    {
        if (duration < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException();
        }

        remaining_time.Value = duration;
        time_elapsed_at_begin = time_internal.Now_Elapsed_Time.Value;
        Continue();
    }

    public void Reset()
    {
        remaining_time.Value = TimeSpan.Zero;
    }

    public void Pause()
    {
        time_internal.Dispose();
    }

    public void Resume()
    {
        Continue();
    }

    void Continue()
    {
        TimeSpan temp_timespan = TimeSpan.Zero;
        TimeSpan dt;
        TimeSpan temp_time_elapsed = TimeSpan.Zero;

        time_internal.Now_Elapsed_Time
            .Subscribe(x => {
                temp_timespan = x - time_elapsed_at_begin;
                dt = temp_timespan - temp_time_elapsed;
                temp_time_elapsed = temp_timespan;
                remaining_time.Value -= dt;
                if (remaining_time.Value <= TimeSpan.Zero)
                {
                    remaining_time.Value = TimeSpan.Zero;
                    on_finished.OnNext(Unit.Default);
                    on_finished.OnCompleted();
                    time_internal.Dispose();
                }
            })
            .AddTo(composite_disposable);
    }

    public void Dispose()
    {
        composite_disposable.Dispose();
    }
}