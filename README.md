Clock application made in Unity

### About interfaces

#ITimeInternal

Provide the application internal elapsed time. Used by others functionalities.

<pre> ```csharp
IReadOnlyReactiveProperty<TimeSpan> Now_Elapsed_Time { get; }
```</pre>

Implemented by:
```TimeInternal```

#ITimeSyncClient

Provide a way for the application to sync its time with an external server.

<pre> ```csharp
UniTask<DateTime?> GetNetworkTimeAsync();
```</pre>

Implemented by:
```NTPTimeSyncClient```

#IClockService

Provide a clock functionality, with Local, Utc and Jst time.

<pre> ```csharp
IReadOnlyReactiveProperty<DateTime> NowUtc { get; }
IReadOnlyReactiveProperty<DateTime> NowJst { get; }
IReadOnlyReactiveProperty<DateTime> NowLocal { get; }
UniTask RefreshAsync();
```</pre>

Implemented by:
```ClockService```

#IStopwatchService

Provide stopwatch functionality

<pre> ```csharp
IReadOnlyReactiveProperty<TimeSpan> ElapsedTime { get; }
IReactiveCollection<TimeSpan> LapTimes { get; }
IReadOnlyReactiveProperty<SWStatus> CurrentState { get; }
IReactiveCommand<Unit> Start { get; }
IReactiveCommand<Unit> Stop { get; }
IReactiveCommand<Unit> Reset { get; }
IReactiveCommand<Unit> Lap { get; }
```</pre>

Implementated by:
```StopwatchService```

#ITimerService

Provide timer functionality

<pre> ```csharp
IReadOnlyReactiveProperty<TimeSpan> RemainingTime { get; }
IReadOnlyReactiveProperty<TStatus> CurrentState { get; }
IReactiveCommand<string> SetTargetTime { get; }
IReactiveCommand<Unit> Start { get; }
IReactiveCommand<Unit> Pause { get; }
IReactiveCommand<Unit> Reset { get; }
IObservable<Unit> OnFinished { get; }
```</pre>

Implementated by:
```TimerService```
	
### CI/CD Workflow

This project uses Github Actions with GameCI for CI/CD.

- On every push or pull request, Unity tests run (editmode, playmode).
- Builds are generated for Windows and uploaded as artifacts.
- Versioning uses Git tags for `MAJOR.MINOR` and commit count for `PATCH`.
- Release builds are auto-generated when a tag is pushed.

### others

You can watch a demo video here

https://drive.google.com/file/d/1SG3CbKD-YSRGouNDUe1f7voU0jDHY-hx/view?usp=sharing

