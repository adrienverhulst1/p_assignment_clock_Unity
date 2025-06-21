using System;
using UniRx;

public interface IStopwatchService
{
    IReadOnlyReactiveProperty<TimeSpan> ElapsedTime { get; }
    IReactiveCollection<TimeSpan> LapTimes { get; }
    void Start();
    void Stop();
    void Reset();
    void Lap();
}