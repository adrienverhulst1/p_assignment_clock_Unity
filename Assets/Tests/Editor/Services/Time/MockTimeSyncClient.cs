// adrien: OK

using System;
using Cysharp.Threading.Tasks;

public class MockTimeSyncClient : ITimeSyncClient
{
    public DateTime? MockTime { get; set; }

    public UniTask<DateTime?> GetNetworkTimeAsync()
    {
        return UniTask.FromResult(MockTime);
    }
}