using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class StopwatchService : IStopwatchService
{
    public IReadOnlyReactiveProperty<TimeSpan> ElapsedTime => throw new NotImplementedException();

    public IReactiveCollection<TimeSpan> LapTimes => throw new NotImplementedException();

    public void Lap()
    {
        throw new NotImplementedException();
    }

    public void Reset()
    {
        throw new NotImplementedException();
    }

    public void Start()
    {
        throw new NotImplementedException();
    }

    public void Stop()
    {
        throw new NotImplementedException();
    }
}
