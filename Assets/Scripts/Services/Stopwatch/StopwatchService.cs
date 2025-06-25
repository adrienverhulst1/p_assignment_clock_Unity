using System;
using System.Collections.Generic;
using UniRx;

public class StopwatchService : IStopwatchService, IDisposable
{
    public IReadOnlyReactiveProperty<SWStatus> CurrentState => current_state;
    private readonly ReactiveProperty<SWStatus> current_state = new ReactiveProperty<SWStatus>(SWStatus.Default);
    public IReadOnlyReactiveProperty<TimeSpan> ElapsedTime => elapsed_time;
    private readonly ReactiveProperty<TimeSpan> elapsed_time = new(TimeSpan.Zero);
    public IReactiveCollection<TimeSpan> LapTimes => lap_times;
    private readonly ReactiveCollection<TimeSpan> lap_times = new(new List<TimeSpan>());

    public IReactiveCommand<Unit> Start { get; }
    public IReactiveCommand<Unit> Stop { get; }
    public IReactiveCommand<Unit> Reset { get; }
    public IReactiveCommand<Unit> Lap { get; }

    private readonly CompositeDisposable composite_disposable = new();

    private TimeSpan time_elapsed_offset = TimeSpan.Zero;
    private TimeSpan time_elapsed_at_pause = TimeSpan.Zero;

    public StopwatchService(ITimeInternal time_internal)
    {
        //ElapsedTime = time_internal.Now_Elapsed_Time
        //    .WithLatestFrom(CurrentState, (now, state) => new { now, state })
        //    .Where(x => x.state == SWStatus.Started)
        //    .Select(x => x.now - time_offset)
        //    .ToReadOnlyReactiveProperty()
        //    .AddTo(composite_disposable);

        time_internal.Now_Elapsed_Time
            .WithLatestFrom(CurrentState, (now, state) => new { now, state })
            .Where(x => x.state == SWStatus.Started)
            .Subscribe(x => {
                elapsed_time.Value = x.now - time_elapsed_offset;
            })
            .AddTo(composite_disposable);

        Start = new ReactiveCommand<Unit>(CurrentState
            .Select(x => x == SWStatus.Default
            || x == SWStatus.Started
            || x == SWStatus.Paused));
        Start
            .WithLatestFrom(CurrentState, (now, state) => new { now, state })
            .Subscribe(x => {
                if(x.state == SWStatus.Started
                || x.state == SWStatus.Default)
                {
                    time_elapsed_offset = time_internal.Now_Elapsed_Time.Value; 
                }
                else if(x.state == SWStatus.Paused)
                {
                    time_elapsed_offset = time_elapsed_offset + (time_internal.Now_Elapsed_Time.Value - time_elapsed_at_pause);
                }
                current_state.Value = SWStatus.Started;
            })
            .AddTo(composite_disposable);

        Stop = new ReactiveCommand<Unit>(CurrentState
            .Select(x => x == SWStatus.Started));
        Stop.Subscribe(_ => {
            time_elapsed_at_pause = time_internal.Now_Elapsed_Time.Value;
            current_state.Value = SWStatus.Paused;
        })
            .AddTo(composite_disposable);

        Reset = new ReactiveCommand<Unit>();
        Reset.Subscribe(_ => {
            current_state.Value = SWStatus.Default;
            elapsed_time.Value = TimeSpan.Zero;
            lap_times.Clear();
        })
            .AddTo(composite_disposable);

        Lap = new ReactiveCommand<Unit>(CurrentState
            .Select(x => x == SWStatus.Started));
        Lap.Subscribe(_ => { 
                lap_times.Add(ElapsedTime.Value);
            })
            .AddTo(composite_disposable);
    }

    public void Dispose()
    {
        composite_disposable.Dispose();
    }
}
