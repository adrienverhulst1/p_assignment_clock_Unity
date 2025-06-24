using System;
using UniRx;

public interface IStopwatchService
{
    IReadOnlyReactiveProperty<TimeSpan> ElapsedTime { get; }
    IReactiveCollection<TimeSpan> LapTimes { get; }
    IReactiveCommand<Unit> Start { get; }
    IReactiveCommand<Unit> Stop { get; }
    IReactiveCommand<Unit> Reset { get; }
    IReactiveCommand<Unit> Lap { get; }
}