using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TimerServicePlayTests
{
    GameObject root_go;
    AppLifetimeScope app;
    TimerUI timer_ui;

    [SetUp]
    public void Setup()
    {
        root_go = new GameObject("root_go");
        app = root_go.AddComponent<AppLifetimeScope>();
        timer_ui = GameObject.FindObjectOfType<TimerUI>();
    }

    [Test]
    public async Task T_Settings()
    {
        timer_ui.InputField.text = "00:10";
        timer_ui.InputField.onEndEdit.Invoke(timer_ui.InputField.text);
        await UniTask.Delay(1000, true);
        //foreach (char c in timer_ui.TimeText.text)
        //{
        //    Debug.Log($"Char: '{c}' - U+{(int)c:X4}");
        //}
        var current_time = TimerService.ParseFlexibleTimeString(timer_ui.TimeText.text);
        Assert.That(current_time, Is.EqualTo(TimeSpan.FromSeconds(10)).Within(TimeSpan.FromMilliseconds(20)));
    }

    [Test]
    public async Task T_Start()
    {
        timer_ui.InputField.text = "00:10";
        timer_ui.InputField.onEndEdit.Invoke(timer_ui.InputField.text);
        timer_ui.PlayButton.onClick.Invoke();
        await UniTask.Delay(1000, true);
        var current_time = TimerService.ParseFlexibleTimeString(timer_ui.TimeText.text);
        Assert.That(current_time, Is.EqualTo(TimeSpan.FromSeconds(9)).Within(TimeSpan.FromMilliseconds(20)));
    }

    [Test]
    public async Task T_StartStop()
    {
        timer_ui.InputField.text = "00:10";
        timer_ui.InputField.onEndEdit.Invoke(timer_ui.InputField.text);
        timer_ui.PlayButton.onClick.Invoke();
        await UniTask.Delay(1000, true);
        timer_ui.PauseButton.onClick.Invoke();
        await UniTask.Delay(1000, true);
        var current_time = TimerService.ParseFlexibleTimeString(timer_ui.TimeText.text);
        Assert.That(current_time, Is.EqualTo(TimeSpan.FromSeconds(9)).Within(TimeSpan.FromMilliseconds(20)));
    }

    [Test]
    public async Task T_StartStopStart()
    {
        timer_ui.InputField.text = "00:10";
        timer_ui.InputField.onEndEdit.Invoke(timer_ui.InputField.text);
        timer_ui.PlayButton.onClick.Invoke();
        await UniTask.Delay(1000, true);
        timer_ui.PauseButton.onClick.Invoke();
        await UniTask.Delay(1000, true);
        timer_ui.PlayButton.onClick.Invoke();
        await UniTask.Delay(1000, true);
        var current_time = TimerService.ParseFlexibleTimeString(timer_ui.TimeText.text);
        Assert.That(current_time, Is.EqualTo(TimeSpan.FromSeconds(8)).Within(TimeSpan.FromMilliseconds(20)));
    }

    [Test]
    public async Task T_Reset()
    {
        timer_ui.InputField.text = "00:10";
        timer_ui.InputField.onEndEdit.Invoke(timer_ui.InputField.text);
        timer_ui.PlayButton.onClick.Invoke();
        await UniTask.Delay(1000, true);
        timer_ui.ResetButton.onClick.Invoke();
        await UniTask.Delay(1000, true);
        var current_time = TimerService.ParseFlexibleTimeString(timer_ui.TimeText.text);
        Assert.That(current_time, Is.EqualTo(TimeSpan.FromSeconds(10)));
    }
}
