using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TimeServicePlayTests
{
    GameObject root_go;
    AppLifetimeScope app;

    [SetUp]
    public void Setup()
    {
        root_go = new GameObject("root_go");
        app = root_go.AddComponent<AppLifetimeScope>();
    }
}
