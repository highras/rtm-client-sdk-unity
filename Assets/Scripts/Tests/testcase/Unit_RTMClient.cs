using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;

using com.fpnn;
using com.rtm;

public class Unit_RTMClient {
    
    [SetUp]
    public void SetUp() {

    }

    [TearDown]
    public void TearDown() {}


    [Test]
    public void Client_ZeroPid() {

        int count = 0;
        Assert.AreEqual(0, count);
    }
}