using System;
using Cysharp.Threading.Tasks;

public interface ITimeSyncClient
{
    UniTask<DateTime?> GetNetworkTimeAsync();
}