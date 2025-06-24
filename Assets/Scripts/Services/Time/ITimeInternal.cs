using System;
using UniRx;

public interface ITimeInternal
{
    IReadOnlyReactiveProperty<TimeSpan> Now_Elapsed_Time { get; }
    IReadOnlyReactiveProperty<DateTime> Now { get; }
}