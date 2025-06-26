using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class StopwatchServicePlayTests
{
    GameObject root_go;
    AppLifetimeScope app;
    StopwatchUI stopwatch_ui;

    [SetUp]
    public void Setup()
    {
        root_go = new GameObject("root_go");
        app = root_go.AddComponent<AppLifetimeScope>();
        stopwatch_ui = GameObject.FindObjectOfType<StopwatchUI>();
    }

    [Test]
    public async Task SW_Start()
    { 
        stopwatch_ui.StartStopButton.onClick.Invoke();
        await UniTask.Delay(1000, true);
        var current_time = TimeSpan.ParseExact(stopwatch_ui.TimeText.text, @"mm\:ss\.ffff", null);
        Assert.That(current_time, Is.EqualTo(TimeSpan.FromSeconds(1)).Within(TimeSpan.FromMilliseconds(20)));
    }

    [Test]
    public async Task SW_StartStop()
    {
        stopwatch_ui.StartStopButton.onClick.Invoke();
        await UniTask.Delay(1000, true);
        stopwatch_ui.StartStopButton.onClick.Invoke();
        await UniTask.Delay(1000, true);
        var current_time = TimeSpan.ParseExact(stopwatch_ui.TimeText.text, @"mm\:ss\.ffff", null);
        Assert.That(current_time, Is.EqualTo(TimeSpan.FromSeconds(1)).Within(TimeSpan.FromMilliseconds(20)));
    }

    [Test]
    public async Task SW_StartStopStart()
    {
        stopwatch_ui.StartStopButton.onClick.Invoke();
        await UniTask.Delay(1000, true);
        stopwatch_ui.StartStopButton.onClick.Invoke();
        await UniTask.Delay(1000, true);
        stopwatch_ui.StartStopButton.onClick.Invoke();
        await UniTask.Delay(1000, true);
        var current_time = TimeSpan.ParseExact(stopwatch_ui.TimeText.text, @"mm\:ss\.ffff", null);
        Assert.That(current_time, Is.EqualTo(TimeSpan.FromSeconds(2)).Within(TimeSpan.FromMilliseconds(20)));
    }

    [Test]
    public async Task SW_StartLap()
    {
        stopwatch_ui.StartStopButton.onClick.Invoke();
        await UniTask.Delay(1000, true);
        stopwatch_ui.LapButton.onClick.Invoke();
        await UniTask.Delay(1000, true);
        var current_time = TimeSpan.ParseExact(stopwatch_ui.LapText.text, @"mm\:ss\.ffff", null);
        Assert.That(current_time, Is.EqualTo(TimeSpan.FromSeconds(1)).Within(TimeSpan.FromMilliseconds(20)));
    }
}
