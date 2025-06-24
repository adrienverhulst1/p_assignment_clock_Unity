using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UniRx;
using UnityEngine;
using UnityEngine.TestTools;

public class StopwatchServiceTests
{
    private ITimeInternal time_internal;

    [SetUp]
    public void Setup()
    {
        time_internal = new TimeInternal();
    }

    [Test]
    public async Task SW_Start()
    {
        StopwatchService sw = new StopwatchService(time_internal);
        await UniTask.Delay(100, true);
        sw.Start.Execute(Unit.Default);
        await UniTask.Delay(1000, true);
        Assert.That(sw.ElapsedTime.Value, Is.EqualTo(TimeSpan.FromSeconds(1.0)).Within(TimeSpan.FromMilliseconds(10)));
    }

    [Test]
    public async Task SW_Stop()
    {
        StopwatchService sw = new StopwatchService(time_internal);
        await UniTask.Delay(100, true);
        sw.Start.Execute(Unit.Default);
        await UniTask.Delay(1000, true);
        sw.Stop.Execute(Unit.Default);
        await UniTask.Delay(1000, true);
        Assert.That(sw.ElapsedTime.Value, Is.EqualTo(TimeSpan.FromSeconds(1.0)).Within(TimeSpan.FromMilliseconds(10)));
    }

    [Test]
    public async Task SW_Reset()
    {
        StopwatchService sw = new StopwatchService(time_internal);
        await UniTask.Delay(100, true);
        sw.Start.Execute(Unit.Default);
        await UniTask.Delay(1000, true);
        sw.Reset.Execute(Unit.Default);
        await UniTask.Delay(1000, true);
        Assert.That(sw.ElapsedTime.Value, Is.EqualTo(TimeSpan.FromSeconds(0)));
    }

    [Test]
    public async Task SW_Lap()
    {
        StopwatchService sw = new StopwatchService(time_internal);
        await UniTask.Delay(100, true);
        sw.Start.Execute(Unit.Default);
        await UniTask.Delay(1000, true);
        sw.Lap.Execute(Unit.Default);
        await UniTask.Delay(1000, true);
        sw.Lap.Execute(Unit.Default);
        await UniTask.Delay(1000, true);
        sw.Lap.Execute(Unit.Default);
        await UniTask.Delay(1000, true);
        Assert.That(sw.LapTimes[0], Is.EqualTo(TimeSpan.FromSeconds(1.0)).Within(TimeSpan.FromMilliseconds(10)));
        Assert.That(sw.LapTimes[1], Is.EqualTo(TimeSpan.FromSeconds(2.0)).Within(TimeSpan.FromMilliseconds(10)));
        Assert.That(sw.LapTimes[2], Is.EqualTo(TimeSpan.FromSeconds(3.0)).Within(TimeSpan.FromMilliseconds(10)));
    }
}
