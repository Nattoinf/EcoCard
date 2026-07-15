using UnityEngine;
using System;
using System.Collections.Generic;

public class GaugeManager : MonoBehaviour
{
    public static GaugeManager Instance { get; private set; }

    [System.Serializable]
    public class Gauge
    {
        public GaugeType type;
        public int max = 100;
        public int current = 50;
    }

    // ★ ここが「Gauges」
    public List<Gauge> Gauges = new List<Gauge>();

    // * ゲージ変更イベント
    public event Action<GaugeType, int> OnGaugeChanged;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // ★ 全ゲージの初期値を UI に反映
        if (GaugeUI.Instance != null)
        {
            GaugeUI.Instance.UpdateAll();
        }
    }


    public void ChangeGauge(GaugeType type, int amount)
    {
        var gauge = Gauges.Find(g => g.type == type);
        if (gauge == null) return;

        int before = gauge.current;

        gauge.current = Mathf.Clamp(
            gauge.current + amount,
            0,
            gauge.max
        );

        // UI 更新
        if (GaugeUI.Instance != null)
        {
            GaugeUI.Instance.UpdateGauge(
                gauge.type,
                gauge.current,
                gauge.max
            );
        }

        // ★ 通知
        if (before != gauge.current)
        {
            OnGaugeChanged?.Invoke(type, gauge.current);
        }
    }
}
