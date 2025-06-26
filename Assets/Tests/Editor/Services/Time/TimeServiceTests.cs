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
        DateTime dt = await mock_clock_service.NowUtc.Skip(1).First().ToTask();
        Assert.That(dt, Is.EqualTo(now + TimeSpan.FromSeconds(30)).Within(TimeSpan.FromMilliseconds(20)));
    }

    [Test]
    public async Task NTP_RefreshAsync_NetworkTimeIsAvailable() // I do not know how to validate this test properly. Here I verify there is a connexion, and NowUtc is still usable.
    {
        NTPTimeSyncClient ntp_time_sync_client = new NTPTimeSyncClient();
        ClockService ntp_clock_service = new ClockService(time_internal, ntp_time_sync_client);
         
        await ntp_clock_service.RefreshAsync();  // somehow the test doesn't wait... TODO
        await UniTask.Delay(2000, true);

        var dt1 = ntp_clock_service.NowUtc.Value;
        await UniTask.Delay(1000, true);
        var dt2 = ntp_clock_service.NowUtc.Value;
        Assert.That(dt2 - dt1, Is.EqualTo(TimeSpan.FromSeconds(1)).Within(TimeSpan.FromMilliseconds(20)));
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
