using UnityEngine;
using UnityEngine.UI; // or TMPro
using VContainer;
using UniRx;
using System;

public class ClockUI : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text time_utc_txt;
    [SerializeField] private TMPro.TMP_Text time_jst_txt;
    [SerializeField] private TMPro.TMP_Text time_local_txt;

    private readonly CompositeDisposable composite_disposable = new();

    [Inject]
    public void Construct(IClockService clock_service)
    {
        clock_service.NowUtc
            .Subscribe(dt => time_utc_txt.text = dt.ToString("HH:mm:ss:ffff"))
            .AddTo(composite_disposable);

        clock_service.NowJst
            .Subscribe(dt => time_jst_txt.text = dt.ToString("HH:mm:ss:ffff"))
            .AddTo(composite_disposable);

        clock_service.NowLocal
            .Subscribe(dt => time_local_txt.text = dt.ToString("HH:mm:ss:ffff"))
            .AddTo(composite_disposable);
    }

    private void OnDestroy()
    {
        composite_disposable.Dispose();
    }
}