using System;
using UniRx;

public interface ITimerService
{
    IReadOnlyReactiveProperty<TimeSpan> RemainingTime { get; }
    void Start(TimeSpan duration);
    void Pause();
    void Resume();
    void Reset();
    IObservable<Unit> OnFinished { get; }
}