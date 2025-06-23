using NUnit.Framework;
using UniRx;
using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

public class TimeServiceTests
{
    private TimeInternal time_internal;

    [SetUp]
    public void Setup()
    {
        time_internal = new TimeInternal();
    }

    [Test]
    public async Task Mock_RefreshAsync_NetworkTimeIsAvailable()
    {
        MockTimeSyncClient mock_time_sync_client = new MockTimeSyncClient();
        ClockService mock_clock_service = new ClockService(time_internal, mock_time_sync_client);

        var now = DateTime.UtcNow;
        mock_time_sync_client.MockTime = now + TimeSpan.FromSeconds(10);
        await mock_clock_service.RefreshAsync();
        DateTime dt = await mock_clock_service.NowUtc.Skip(1).First().ToTask();
        Assert.That(dt, Is.EqualTo(now + TimeSpan.FromSeconds(10)).Within(TimeSpan.FromMilliseconds(10)));
    }

    [Test]
    public async Task NTP_RefreshAsync_NetworkTimeIsAvailable()
    {
        NTPTimeSyncClient ntp_time_sync_client = new NTPTimeSyncClient();
        ClockService ntp_clock_service = new ClockService(time_internal, ntp_time_sync_client);

        var now = DateTime.UtcNow;
        await ntp_clock_service.RefreshAsync();
        await Task.Delay(1000);
        DateTime dt = await ntp_clock_service.NowUtc.Skip(1).First().ToTask();
        Assert.That(dt, Is.EqualTo(now + TimeSpan.FromSeconds(1)).Within(TimeSpan.FromSeconds(1)));
    }

    [Test]
    public void NTP_JstConversion()
    {
        NTPTimeSyncClient ntp_time_sync_client = new NTPTimeSyncClient();
        ClockService ntp_clock_service = new ClockService(time_internal, ntp_time_sync_client);

        Assert.AreEqual(ntp_clock_service.NowUtc.Value + TimeSpan.FromHours(9), ntp_clock_service.NowJst.Value);
    }

    [Test]
    public void NTP_LocalConversion()
    {
        NTPTimeSyncClient ntp_time_sync_client = new NTPTimeSyncClient();
        ClockService ntp_clock_service = new ClockService(time_internal, ntp_time_sync_client);

        Assert.AreEqual(ntp_clock_service.NowUtc.Value + TimeZoneInfo.Local.BaseUtcOffset, ntp_clock_service.NowLocal.Value);
    }
}
