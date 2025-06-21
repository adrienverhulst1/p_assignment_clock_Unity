using UnityEngine;
using VContainer;
using VContainer.Unity;

public class AppLifetimeScope : LifetimeScope
{
    [SerializeField]
    TimeUI time_ui;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<NTPTimeSyncClient>(Lifetime.Singleton).As<ITimeSyncClient>();
        builder.Register<TimeService>(Lifetime.Singleton).As<ITimeService>();

        builder.RegisterComponent(time_ui);
    }
}
