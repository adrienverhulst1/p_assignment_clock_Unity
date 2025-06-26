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
    public async Task Timer_SetTarget()
    {
        TimerService timer = new TimerService(time_internal);
        await UniTask.Delay(100, true);
        timer.SetTargetTime.Execute("10");
        Assert.That(timer.RemainingTime.Value, Is.EqualTo(TimeSpan.FromSeconds(10.0)));
    }

    [Test]
    public async Task Timer_Start()
    {
        TimerService timer = new TimerService(time_internal);
        await UniTask.Delay(100, true);
        timer.SetTargetTime.Execute("10");
        timer.Start.Execute(Unit.Default);
        await UniTask.Delay(1000, true);
        Assert.That(timer.RemainingTime.Value, Is.EqualTo(TimeSpan.FromSeconds(9.0)).Within(TimeSpan.FromMilliseconds(20)));
    }

    [Test]
    public async Task Timer_Pause()
    {
        TimerService timer = new TimerService(time_internal);
        await UniTask.Delay(100, true);
        timer.SetTargetTime.Execute("10");
        timer.Start.Execute(Unit.Default);
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
        timer.SetTargetTime.Execute("10");
        timer.Start.Execute(Unit.Default);
        await UniTask.Delay(1000, true); 
        timer.Pause.Execute(Unit.Default);
        await UniTask.Delay(1000, true);
        timer.Start.Execute(Unit.Default);
        await UniTask.Delay(1000, true);
        Assert.That(timer.RemainingTime.Value, Is.EqualTo(TimeSpan.FromSeconds(8.0)).Within(TimeSpan.FromMilliseconds(20)));
    }

    [Test]
    public async Task Timer_Reset()
    {
        TimerService timer = new TimerService(time_internal);
        await UniTask.Delay(100, true);
        timer.SetTargetTime.Execute("10");
        timer.Start.Execute(Unit.Default);
        await UniTask.Delay(1000, true);
        timer.Reset.Execute(Unit.Default);
        Assert.That(timer.RemainingTime.Value, Is.EqualTo(TimeSpan.FromSeconds(10)));
    }

    [Test]
    public async Task SW_UseCase1()
    {
        // Set -> Start -> Wait to end
        TimerService timer = new TimerService(time_internal);
        await UniTask.Delay(100, true);

        var tcs = new UniTaskCompletionSource();

        timer.OnFinished.Subscribe(_ =>
        {
            Assert.That(timer.RemainingTime.Value, Is.EqualTo(TimeSpan.FromSeconds(0)));
            tcs.TrySetResult();
        });

        timer.SetTargetTime.Execute("2");
        timer.Start.Execute(Unit.Default);

        await tcs.Task.Timeout(TimeSpan.FromSeconds(3));
    }

    [Test]
    public async Task SW_UseCase2()
    {
        // Set -> Start -> Wait to end (too short)
        TimerService timer = new TimerService(time_internal);
        await UniTask.Delay(100, true);

        bool flag = false;

        timer.OnFinished.Subscribe(_ =>
        {
            flag = true;
        });

        timer.SetTargetTime.Execute("2");
        timer.Start.Execute(Unit.Default);

        await UniTask.Delay(1000, true);

        Assert.IsFalse(flag);
    }
}
