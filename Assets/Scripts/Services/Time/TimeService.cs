using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UniRx;

public sealed class TimeService : ITimeService, IDisposable
{
    private readonly ITimeSyncClient time_sync_client;
    readonly ReactiveProperty<DateTime> now = new(DateTime.UtcNow);

    readonly CompositeDisposable composite_disposable = new();
    TimeSpan timespan_offset = TimeSpan.Zero;
    private CancellationTokenSource cancel_token_source = new();

    public TimeService(ITimeSyncClient time_sync_client)
    {
        this.time_sync_client = time_sync_client;
        now.AddTo(composite_disposable);

        NowJst = now
            .Select(x => x + TimeSpan.FromHours(9))
            .ToReadOnlyReactiveProperty()
            .AddTo(composite_disposable);

        NowLocal = now
            .Select(x => x.ToLocalTime())
            .ToReadOnlyReactiveProperty()
            .AddTo(composite_disposable);

        StartTicking().Forget();
    }

    private async UniTaskVoid StartTicking()
    {
        var token = cancel_token_source.Token;
        while (token.IsCancellationRequested == false)
        {
            now.Value = DateTime.UtcNow + timespan_offset;
            await UniTask.Delay(1, 
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

    public IReadOnlyReactiveProperty<DateTime> NowUtc => now;
    public IReadOnlyReactiveProperty<DateTime> NowJst { get; }
    public IReadOnlyReactiveProperty<DateTime> NowLocal { get; }

    public async UniTask RefreshAsync()
    {
        var temp_network_time = await time_sync_client.GetNetworkTimeAsync();
        if (temp_network_time.HasValue) timespan_offset = temp_network_time.Value - DateTime.UtcNow;
        else throw new Exception($"RefreshAsync {temp_network_time} {temp_network_time.HasValue}");
    }
}