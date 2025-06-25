using UnityEngine;
using VContainer;
using VContainer.Unity;

public enum SWStatus
{
    Default,
    Started,
    Paused
}

public enum TStatus
{
    Started,
    Paused,
    Stopped
}

public class AppLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        //builder.RegisterComponentInNewPrefab<ManagerUI>(
        //    manager_prefab,
        //    Lifetime.Singleton
        //);

        builder.Register<TimeInternal>(Lifetime.Singleton).As<ITimeInternal>();
        builder.Register<NTPTimeSyncClient>(Lifetime.Singleton).As<ITimeSyncClient>();
        builder.Register<ClockService>(Lifetime.Singleton).As<IClockService>();
        builder.Register<TimerService>(Lifetime.Singleton).As<ITimerService>();
        builder.Register<StopwatchService>(Lifetime.Singleton).As<IStopwatchService>();

        ManagerUI manager_prefab = Resources.Load<ManagerUI>("Main UI");
        var manager_go = Instantiate(manager_prefab.gameObject);
        builder.RegisterComponent(manager_go.GetComponent<ManagerUI>());
        builder.RegisterComponent(manager_go.GetComponent<ClockUI>());
        builder.RegisterComponent(manager_go.GetComponent<TimerUI>());
        builder.RegisterComponent(manager_go.GetComponent<StopwatchUI>());
        builder.RegisterComponent(manager_go.GetComponent<PanelNavigationUI>());

        builder.RegisterEntryPoint<RootInitializer>();
    }
}

public class RootInitializer : IInitializable
{
    readonly ManagerUI manager_ui;

    public RootInitializer(ManagerUI manager_ui)
    {
        this.manager_ui = manager_ui;
    }

    public void Initialize()
    {

    }
}