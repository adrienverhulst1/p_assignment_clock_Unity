using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class NavigationPlayTest
{
    GameObject root_go;
    AppLifetimeScope app;
    PanelNavigationUI navigation_ui;

    [SetUp]
    public void Setup()
    {
        root_go = new GameObject("root_go");
        app = root_go.AddComponent<AppLifetimeScope>();
        navigation_ui = GameObject.FindObjectOfType<PanelNavigationUI>();
    }

    [Test]
    public void AfterSetup()
    {
        Assert.AreSame("Timer", navigation_ui.Title.text);
    }

    [Test]
    public void GoToStopwatch()
    {
        navigation_ui.StopwatchButton.onClick.Invoke();
        Assert.AreSame("Stopwatch", navigation_ui.Title.text);
    }

    [Test]
    public void GoToTimer()
    {
        navigation_ui.TimerButton.onClick.Invoke();
        Assert.AreSame("Timer", navigation_ui.Title.text);
    }
}
