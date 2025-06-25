using System;
using UniRx;

public interface ITimerService
{
    IReadOnlyReactiveProperty<TimeSpan> RemainingTime { get; }
    IReadOnlyReactiveProperty<TStatus> CurrentState { get; }
    IReactiveCommand<string> SetTargetTime { get; }
    IReactiveCommand<Unit> Start { get; }
    IReactiveCommand<Unit> Pause { get; }
    IReactiveCommand<Unit> Resume { get; }
    IReactiveCommand<Unit> Reset { get; }
    IObservable<Unit> OnFinished { get; }
}