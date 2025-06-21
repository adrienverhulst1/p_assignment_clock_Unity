using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class TimerService : ITimerService
{
    public IReadOnlyReactiveProperty<TimeSpan> RemainingTime => throw new NotImplementedException();

    public void Pause()
    {
        throw new NotImplementedException();
    }

    public void Reset()
    {
        throw new NotImplementedException();
    }

    public void Resume()
    {
        throw new NotImplementedException();
    }

    public void Start(TimeSpan duration)
    {
        throw new NotImplementedException();
    }
}
