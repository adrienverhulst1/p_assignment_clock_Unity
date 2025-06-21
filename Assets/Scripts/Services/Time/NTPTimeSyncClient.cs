using System;
using System.Net.Sockets;
using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class NTPTimeSyncClient : ITimeSyncClient
{
    // https://stackoverflow.com/questions/1193955/how-to-query-an-ntp-server-using-c
    public async UniTask<DateTime?> GetNetworkTimeAsync()
    {
        try
        {
            using var udp = new UdpClient("pool.ntp.org", 123);
            var diagram = new byte[48]; diagram[0] = 0x1B;
            await udp.SendAsync(diagram, diagram.Length);
            var response = await udp.ReceiveAsync().AsUniTask();

            ulong int_part = BitConverter.ToUInt32(response.Buffer, 40).SwapEndianness();
            ulong fract_part = BitConverter.ToUInt32(response.Buffer, 44).SwapEndianness();
            var ms = (int_part * 1000) + ((fract_part * 1000) / 0x100000000L);
            return new DateTime(1900, 1, 1).AddMilliseconds(ms);
        }
        catch (Exception e)
        {
            throw e;
        }
    }
}