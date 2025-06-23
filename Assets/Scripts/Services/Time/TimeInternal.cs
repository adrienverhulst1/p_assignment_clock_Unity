using System.Threading;
using System;
using UniRx;
using Cysharp.Threading.Tasks;
using System.Diagnostics;

public class TimeInternal : IDisposable
{
    public IReadOnlyReactiveProperty<TimeSpan> Now_Elapsed_Time => now_elapsed_time;
    public IReadOnlyReactiveProperty<DateTime> Now { get; }
    readonly ReactiveProperty<TimeSpan> now_elapsed_time = new(TimeSpan.Zero);
    readonly CompositeDisposable composite_disposable = new();

    private CancellationTokenSource cancel_token_source = new();
    Stopwatch stopwatch;

    public TimeInternal()
    {
        Now = Now_Elapsed_Time
            .Select(x => DateTime.UtcNow + x)
            .ToReadOnlyReactiveProperty()
            .AddTo(composite_disposable);

        StartTicking().Forget();
    }

    private async UniTaskVoid StartTicking()
    {
        stopwatch = Stopwatch.StartNew();

        var token = cancel_token_source.Token;
        while (token.IsCancellationRequested == false)
        {
            now_elapsed_time.Value = stopwatch.Elapsed;
            await UniTask.Delay(10,
                DelayType.Realtime,
                PlayerLoopTiming.Update,
                token);
        }
    }

    public void Dispose()
    {
        cancel_token_source.Cancel();
        cancel_token_source.Dispose();
        composite_disposable.Dispose();
    }
}