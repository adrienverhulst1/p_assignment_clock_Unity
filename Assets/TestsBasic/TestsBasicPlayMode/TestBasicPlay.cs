using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestBasicPlay
{
    // A Test behaves as an ordinary method
    [Test]
    public void TestBasicPlaySimplePasses1()
    {
        Assert.IsTrue(Application.isPlaying);
    }
}
