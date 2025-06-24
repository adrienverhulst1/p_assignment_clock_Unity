using System;
using UniRx;
using UnityEngine.UIElements;

public class StopwatchService : IStopwatchService, IDisposable
{
    public enum SWStatus
    {
        Default,
        Started,
        Stopped
    }

    public IReadOnlyReactiveProperty<SWStatus> CurrentState => current_state;
    private readonly ReactiveProperty<SWStatus> current_state = new ReactiveProperty<SWStatus>(SWStatus.Default);
    public IReadOnlyReactiveProperty<TimeSpan> ElapsedTime { get; }
    public IReactiveCollection<TimeSpan> LapTimes => lap_times;
    private readonly ReactiveCollection<TimeSpan> lap_times;

    public IReactiveCommand<Unit> Start { get; }
    public IReactiveCommand<Unit> Stop { get; }
    public IReactiveCommand<Unit> Reset { get; }
    public IReactiveCommand<Unit> Lap { get; }

    private readonly CompositeDisposable composite_disposable = new();

    private TimeSpan time_elapsed_at_begin = TimeSpan.Zero;

    public StopwatchService(ITimeInternal time_internal)
    {
        ElapsedTime = time_internal.Now_Elapsed_Time
            .WithLatestFrom(CurrentState, (now, state) => new { now, state })
            .Where(x => x.state == SWStatus.Started)
            .Select(x => x.now - time_elapsed_at_begin)
            .ToReadOnlyReactiveProperty()
            .AddTo(composite_disposable);

        Start = new ReactiveCommand<Unit>(CurrentState
            .Select(x => x == SWStatus.Default
            || x == SWStatus.Started
            || x == SWStatus.Stopped));
        Start.Subscribe(_ => {
            time_elapsed_at_begin = time_internal.Now_Elapsed_Time.Value;
            current_state.Value = SWStatus.Started;
        })
            .AddTo(composite_disposable); ;

        Stop = new ReactiveCommand<Unit>(CurrentState
            .Select(x => x == SWStatus.Started));
        Stop.Subscribe(_ => {
            current_state.Value = SWStatus.Stopped;
        })
            .AddTo(composite_disposable); ;

        Reset = new ReactiveCommand<Unit>();
        Reset.Subscribe(_ => {
            current_state.Value = SWStatus.Default;
        })
            .AddTo(composite_disposable); ;

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
