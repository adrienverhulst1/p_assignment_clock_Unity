using System;
using UniRx;

public interface ITimerService
{
    IReadOnlyReactiveProperty<TimeSpan> RemainingTime { get; }
    IReadOnlyReactiveProperty<int> CurrentState { get; }
    void Start(TimeSpan duration);
    void Pause();
    void Resume();
    void Reset();
    IObservable<Unit> OnFinished { get; }
}