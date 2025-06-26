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
    DateTime network_time = DateTime.UtcNow;

    public ClockService(ITimeInternal time_internal, ITimeSyncClient time_sync_client)
    {
        this.time_sync_client = time_sync_client;

        NowUtc = time_internal.Now_Elapsed_Time
            .Select(x => network_time + x)
            .ToReadOnlyReactiveProperty()
            .AddTo(composite_disposable);

        NowJst = time_internal.Now_Elapsed_Time
            .Select(x => network_time + TimeSpan.FromHours(9) + x)
            .ToReadOnlyReactiveProperty()
            .AddTo(composite_disposable);

        NowLocal = time_internal.Now_Elapsed_Time
            .Select(x => network_time.ToLocalTime() + x)
            .ToReadOnlyReactiveProperty()
            .AddTo(composite_disposable);

        RefreshAsync().Forget(ex => UnityEngine.Debug.LogError($"RefreshAsync failed: {ex}")); // TODO

        Observable.Interval(TimeSpan.FromDays(1)).Subscribe(_ => {
            RefreshAsync().Forget(ex => UnityEngine.Debug.LogError($"RefreshAsync failed: {ex}")); // TODO
        });
    }

    public void Dispose()
    {
        composite_disposable.Dispose();
    }

    public async UniTask RefreshAsync()
    {
        var temp_network_time = await time_sync_client.GetNetworkTimeAsync(TimeSpan.FromSeconds(3));
        if (temp_network_time.HasValue) network_time = temp_network_time.Value;
        else throw new Exception($"RefreshAsync {temp_network_time} {temp_network_time.HasValue}");
    }
}