using System.Threading;
using System;
using UniRx;
using Cysharp.Threading.Tasks;
using System.Diagnostics;

public class TimeInternal : ITimeInternal, IDisposable
{
    public IReadOnlyReactiveProperty<TimeSpan> Now_Elapsed_Time => now_elapsed_time;
    readonly ReactiveProperty<TimeSpan> now_elapsed_time = new(TimeSpan.Zero);
    readonly CompositeDisposable composite_disposable = new();

    private CancellationTokenSource cancel_token_source = new(); // cannot be accessed
    Stopwatch stopwatch;

    public TimeInternal()
    {
        StartTicking().Forget();
    }

    private async UniTaskVoid StartTicking()
    {
        stopwatch = Stopwatch.StartNew();
        var token = cancel_token_source.Token;
        var nextTick = stopwatch.Elapsed;

        try
        {
            while (token.IsCancellationRequested == false)
            {
                now_elapsed_time.Value = stopwatch.Elapsed;

                nextTick += TimeSpan.FromMilliseconds(10);
                var delay = nextTick - stopwatch.Elapsed;

                if (delay > TimeSpan.Zero) await UniTask.Delay(delay, DelayType.Realtime, PlayerLoopTiming.Update, token);
                else await UniTask.Yield(PlayerLoopTiming.Update);
            }
        }
        catch (OperationCanceledException e)
        {
            // ... TODO
        }
    }

    public void Dispose()
    {
        cancel_token_source.Cancel();
        cancel_token_source.Dispose();
        composite_disposable.Dispose();
    }
}