using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UniRx;
using UnityEngine;
using UnityEngine.TestTools;

public class TimerServiceTests
{
    private TimeInternal time_internal;

    [SetUp]
    public void Setup()
    {
        time_internal = new TimeInternal();
    }

    [Test]
    public async Task Timer_Start()
    {
        TimerService timer = new TimerService(time_internal);
        await UniTask.Delay(100, true);
        timer.Start.Execute(TimeSpan.FromSeconds(10.0));
        await UniTask.Delay(1000, true);
        Assert.That(timer.RemainingTime.Value, Is.EqualTo(TimeSpan.FromSeconds(9.0)).Within(TimeSpan.FromMilliseconds(20)));
    }

    [Test]
    public async Task Timer_Pause()
    {
        TimerService timer = new TimerService(time_internal);
        await UniTask.Delay(100, true);
        timer.Start.Execute(TimeSpan.FromSeconds(10.0));
        await UniTask.Delay(1000, true);
        timer.Pause.Execute(Unit.Default);
        await UniTask.Delay(1000, true);
        Assert.That(timer.RemainingTime.Value, Is.EqualTo(TimeSpan.FromSeconds(9.0)).Within(TimeSpan.FromMilliseconds(20)));
    }

    [Test]
    public async Task Timer_Resume()
    {
        TimerService timer = new TimerService(time_internal);
        await UniTask.Delay(100, true);
        timer.Start.Execute(TimeSpan.FromSeconds(10.0));
        await UniTask.Delay(1000, true);
        timer.Pause.Execute(Unit.Default);
        await UniTask.Delay(1000, true);
        timer.Resume.Execute(Unit.Default);
        await UniTask.Delay(1000);
        Assert.That(timer.RemainingTime.Value, Is.EqualTo(TimeSpan.FromSeconds(8.0)).Within(TimeSpan.FromMilliseconds(20)));
    }

    [Test]
    public async Task Timer_Reset()
    {
        TimerService timer = new TimerService(time_internal);
        await UniTask.Delay(100, true);
        timer.Start.Execute(TimeSpan.FromSeconds(10.0));
        await UniTask.Delay(1000, true);
        timer.Reset.Execute(Unit.Default);
        Assert.That(timer.RemainingTime.Value, Is.EqualTo(TimeSpan.FromSeconds(0)));
    }
}
