using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
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
        await Task.Delay(1000);
        timer.Start(TimeSpan.FromSeconds(10.0));
        await Task.Delay(1000);
        Assert.That(timer.RemainingTime.Value, Is.EqualTo(TimeSpan.FromSeconds(9.0)).Within(TimeSpan.FromMilliseconds(100)));
    }

    [Test]
    public async Task Timer_Pause()
    {
        TimerService timer = new TimerService(time_internal);
        await Task.Delay(1000);
        timer.Start(TimeSpan.FromSeconds(10.0));
        await Task.Delay(1000);
        timer.Pause();
        await Task.Delay(1000);
        Assert.That(timer.RemainingTime.Value, Is.EqualTo(TimeSpan.FromSeconds(9.0)).Within(TimeSpan.FromMilliseconds(100)));
    }

    [Test]
    public async Task Timer_Resume()
    {
        TimerService timer = new TimerService(time_internal);
        await Task.Delay(1000);
        timer.Start(TimeSpan.FromSeconds(10.0));
        await Task.Delay(1000);
        timer.Pause();
        await Task.Delay(1000);
        timer.Resume();
        await Task.Delay(1000);
        Assert.That(timer.RemainingTime.Value, Is.EqualTo(TimeSpan.FromSeconds(8.0)).Within(TimeSpan.FromMilliseconds(100)));
    }

    [Test]
    public async Task Timer_Reset()
    {
        TimerService timer = new TimerService(time_internal);
        await Task.Delay(1000);
        timer.Start(TimeSpan.FromSeconds(10.0));
        await Task.Delay(1000);
        timer.Reset();
        Assert.That(timer.RemainingTime.Value, Is.EqualTo(TimeSpan.FromSeconds(0)).Within(TimeSpan.FromMilliseconds(20)));
    }
}
