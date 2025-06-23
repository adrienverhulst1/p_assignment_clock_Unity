using UnityEngine;
using VContainer;
using VContainer.Unity;

public class AppLifetimeScope : LifetimeScope
{
    [SerializeField]
    ClockUI clock_ui;
    [SerializeField]
    TimerUI timer_ui;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<TimeInternal>(Lifetime.Singleton).As<TimeInternal>();
        builder.Register<NTPTimeSyncClient>(Lifetime.Singleton).As<ITimeSyncClient>();
        builder.Register<ClockService>(Lifetime.Singleton).As<IClockService>();
        builder.Register<TimerService>(Lifetime.Singleton).As<ITimerService>();

        builder.RegisterComponent(clock_ui);
        builder.RegisterComponent(timer_ui);
    }
}
