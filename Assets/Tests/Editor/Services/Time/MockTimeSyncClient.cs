// adrien: OK

using System;
using Cysharp.Threading.Tasks;

public class MockTimeSyncClient : ITimeSyncClient
{
    public DateTime? MockTime { get; set; }

    public UniTask<DateTime?> GetNetworkTimeAsync(TimeSpan timeout)
    {
        MockTime = DateTime.UtcNow.AddSeconds(30);
        return UniTask.FromResult(MockTime);
    }
}