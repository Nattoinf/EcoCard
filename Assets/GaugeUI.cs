using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GaugeUI : MonoBehaviour
{
    public static GaugeUI Instance { get; private set; }

    [System.Serializable]
    public class GaugeView
    {
        public GaugeType type;
        public Image fillImage;
    }

    public List<GaugeView> gaugeViews = new List<GaugeView>();

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
        UpdateAll();
    }

    public void UpdateGauge(GaugeType type, int current, int max)
    {
        var view = gaugeViews.Find(v => v.type == type);
        if (view == null || view.fillImage == null) return;

        view.fillImage.fillAmount = (float)current / max;
    }

    public void UpdateAll()
    {
        if (GaugeManager.Instance == null) return;

        foreach (var g in GaugeManager.Instance.Gauges)
        {
            UpdateGauge(g.type, g.current, g.max);
        }
    }
}
