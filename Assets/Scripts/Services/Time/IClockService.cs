using Cysharp.Threading.Tasks;
using System;
using UniRx;

public interface IClockService
{
    IReadOnlyReactiveProperty<DateTime> NowUtc { get; }
    IReadOnlyReactiveProperty<DateTime> NowJst { get; }
    IReadOnlyReactiveProperty<DateTime> NowLocal { get; }
    UniTask RefreshAsync();
}