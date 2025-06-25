using Palmmedia.ReportGenerator.Core.Common;
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

    private TimeSpan time_elapsed_offset = TimeSpan.Zero;
    private TimeSpan time_elapsed_at_pause;
    private TimeSpan target_time;

    public IReactiveCommand<string> SetTargetTime { get; }
    //public IReactiveCommand<TimeSpan> Start { get; }
    public IReactiveCommand<Unit> Start { get; }
    public IReactiveCommand<Unit> Pause { get; }
    public IReactiveCommand<Unit> Resume { get; }
    public IReactiveCommand<Unit> Reset { get; }

    public static TimeSpan ParseFlexibleTimeString(string input)
    {
        input = input.Trim().Replace('：', ':').Replace("\u00A0", "").Replace("\u200B", "");
        var timespan = TimeSpan.Zero;
        if (TimeSpan.TryParseExact(input, @"mm\:ss", null, out timespan))
        {
            return timespan;
        }
        if (TimeSpan.TryParseExact(input, @"m\:ss", null, out timespan))
        {
            return timespan;
        }
        if (TimeSpan.TryParseExact(input, @"\:ss", null, out timespan))
        {
            return timespan;
        }
        if (int.TryParse(input, out int seconds))
        {
            return TimeSpan.FromSeconds(seconds);
        }
        UnityEngine.Debug.LogWarning($"Failed to parse {input}");
        return TimeSpan.Zero;
    }

    public TimerService(ITimeInternal time_internal)
    {
        TimeSpan temp_timespan = TimeSpan.Zero;
        int rounded_seconds = 0;
        time_internal.Now_Elapsed_Time
            .WithLatestFrom(CurrentState, (now, state) => new { now, state })
            .Where(x => x.state == TStatus.Started)
            .Subscribe(x => {
                temp_timespan = target_time - (x.now - time_elapsed_offset);
                rounded_seconds = (int)Math.Round(temp_timespan.TotalSeconds);
                remaining_time.Value = TimeSpan.FromSeconds(rounded_seconds);
                //UnityEngine.Debug.LogWarning($"{remaining_time.Value}");

                if (remaining_time.Value <= TimeSpan.Zero)
                {
                    current_state.Value = 0;
                    remaining_time.Value = TimeSpan.Zero;
                    on_finished.OnNext(Unit.Default);
                    
                    Reset.Execute(Unit.Default);
                    //on_finished.OnCompleted();
                }
            })
            .AddTo(composite_disposable);

        SetTargetTime = new ReactiveCommand<string>(CurrentState
            .Select(x => x == TStatus.Started 
            || x == TStatus.Paused
            || x == TStatus.Stopped));
        SetTargetTime
            .WithLatestFrom(CurrentState, (now, state) => new { now, state })
            //.Throttle(TimeSpan.FromMilliseconds(200))
            .Subscribe(x =>
            {
                if(x.state == TStatus.Started
                || x.state == TStatus.Paused) 
                {
                    current_state.Value = TStatus.Stopped;
                }

                target_time = ParseFlexibleTimeString(x.now);
                remaining_time.Value = target_time;
            })
            .AddTo(composite_disposable);

        Start = new ReactiveCommand<Unit>(CurrentState
            .Select(x => x == TStatus.Stopped
            || x == TStatus.Paused));
        Start
            .WithLatestFrom(CurrentState, (now, state) => new { now, state })
            .Subscribe(x => {
                if(x.state == TStatus.Stopped)
                {
                    UnityEngine.Debug.LogWarning($"{time_internal.Now_Elapsed_Time.Value}");
                    time_elapsed_offset = time_internal.Now_Elapsed_Time.Value;
                }
                else if (x.state == TStatus.Paused)
                {
                    time_elapsed_offset = time_elapsed_offset + (time_internal.Now_Elapsed_Time.Value - time_elapsed_at_pause);
                }

                current_state.Value = TStatus.Started;
            })
            .AddTo(composite_disposable);

        Pause = new ReactiveCommand<Unit>(CurrentState
            .Select(x => x == TStatus.Started));
        Pause
            .WithLatestFrom(CurrentState, (now, state) => new { now, state })
            .Where(x => x.state == TStatus.Started)
            .Subscribe(x => {
                time_elapsed_at_pause = time_internal.Now_Elapsed_Time.Value;
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
            .Select(x => x == TStatus.Started
            || x == TStatus.Paused));
        Reset.WithLatestFrom(CurrentState, (now, state) => new { now, state })
            .Subscribe(x => {
                current_state.Value = TStatus.Stopped; 
                remaining_time.Value = target_time;
            })
            .AddTo(composite_disposable);
    }

    public void Dispose()
    {
        composite_disposable.Dispose();
    }
}