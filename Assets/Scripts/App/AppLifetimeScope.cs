using UnityEngine;
using VContainer;
using VContainer.Unity;

public enum SWStatus
{
    Default,
    Started,
    Stopped
}

public enum TStatus
{
    Started,
    Paused,
    Stopped
}

public class AppLifetimeScope : LifetimeScope
{
    [SerializeField]
    ClockUI clock_ui;
    [SerializeField]
    TimerUI timer_ui;
    [SerializeField]
    StopwatchUI stopwatch_ui;
    [SerializeField]
    PanelNavigationUI panel_navigation_ui;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<TimeInternal>(Lifetime.Singleton).As<ITimeInternal>();
        builder.Register<NTPTimeSyncClient>(Lifetime.Singleton).As<ITimeSyncClient>();
        builder.Register<ClockService>(Lifetime.Singleton).As<IClockService>();
        builder.Register<TimerService>(Lifetime.Singleton).As<ITimerService>();
        builder.Register<StopwatchService>(Lifetime.Singleton).As<IStopwatchService>();

        builder.RegisterComponent(clock_ui);
        builder.RegisterComponent(timer_ui);
        builder.RegisterComponent(stopwatch_ui);
        builder.RegisterComponent(panel_navigation_ui);
    }
}
