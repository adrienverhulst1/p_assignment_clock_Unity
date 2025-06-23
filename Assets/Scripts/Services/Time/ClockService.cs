using Cysharp.Threading.Tasks;
using System;
using UniRx;

public class ClockService : IClockService, IDisposable
{
    public IReadOnlyReactiveProperty<DateTime> NowUtc { get; }
    public IReadOnlyReactiveProperty<DateTime> NowJst { get; }
    public IReadOnlyReactiveProperty<DateTime> NowLocal { get; }

    readonly CompositeDisposable composite_disposable = new();

    private readonly ITimeSyncClient time_sync_client;
    TimeSpan time_network_offset = TimeSpan.Zero;

    public ClockService(TimeInternal time_internal, ITimeSyncClient time_sync_client)
    {
        this.time_sync_client = time_sync_client;

        NowUtc = time_internal.Now
            .Select(x => x + time_network_offset)
            .ToReadOnlyReactiveProperty()
            .AddTo(composite_disposable);

        NowJst = time_internal.Now
            .Select(x => x + time_network_offset + TimeSpan.FromHours(9))
            .ToReadOnlyReactiveProperty()
            .AddTo(composite_disposable);

        NowLocal = time_internal.Now
            .Select(x => x.ToLocalTime())
            .ToReadOnlyReactiveProperty()
            .AddTo(composite_disposable);
    }

    public void Dispose()
    {
        composite_disposable.Dispose();
    }

    public async UniTask RefreshAsync()
    {
        var temp_network_time = await time_sync_client.GetNetworkTimeAsync();
        if (temp_network_time.HasValue) time_network_offset = temp_network_time.Value - DateTime.UtcNow;
        else throw new Exception($"RefreshAsync {temp_network_time} {temp_network_time.HasValue}");
    }
}