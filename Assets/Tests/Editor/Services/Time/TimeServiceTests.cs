using NUnit.Framework;
using UniRx;
using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

public class TimeServiceTests
{
    private MockTimeSyncClient mock_time_sync_client;
    private TimeService mock_time_service;
    private NTPTimeSyncClient ntp_time_sync_client;
    private TimeService ntp_time_service;

    [SetUp]
    public void Setup()
    {
        mock_time_sync_client = new MockTimeSyncClient();
        mock_time_service = new TimeService(mock_time_sync_client);
        ntp_time_sync_client = new NTPTimeSyncClient();
        ntp_time_service = new TimeService(ntp_time_sync_client);
    }

    [Test]
    public async Task Mock_RefreshAsync_NetworkTimeIsAvailable()
    {
        var now = DateTime.UtcNow;
        mock_time_sync_client.MockTime = now + TimeSpan.FromSeconds(10);
        await mock_time_service.RefreshAsync();
        DateTime dt = await mock_time_service.NowUtc.Skip(1).First().ToTask();
        Assert.That(dt, Is.EqualTo(now + TimeSpan.FromSeconds(10)).Within(TimeSpan.FromMilliseconds(10)));
    }

    [Test]
    public async Task NTP_RefreshAsync_NetworkTimeIsAvailable()
    {
        var now = DateTime.UtcNow;
        await ntp_time_service.RefreshAsync();
        await Task.Delay(1000);
        DateTime dt = await ntp_time_service.NowUtc.Skip(1).First().ToTask();
        Assert.That(dt, Is.EqualTo(now + TimeSpan.FromSeconds(1)).Within(TimeSpan.FromSeconds(1)));
    }

    [Test]
    public void NTP_JstConversion()
    {
        Assert.AreEqual(ntp_time_service.NowUtc.Value + TimeSpan.FromHours(9), ntp_time_service.NowJst.Value);
    }

    [Test]
    public void NTP_LocalConversion()
    {
        Assert.AreEqual(ntp_time_service.NowUtc.Value + TimeZoneInfo.Local.BaseUtcOffset, ntp_time_service.NowLocal.Value);
    }
}
