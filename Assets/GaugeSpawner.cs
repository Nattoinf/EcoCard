using UnityEngine;
using System.Collections.Generic;

public class GaugeSpawner : MonoBehaviour
{
    public GaugeType gaugeType;

    public GameObject spawnPrefab;
    public Transform spawnCenter;
    public float radius = 2f;

    private List<GameObject> spawnedObjects = new List<GameObject>();

    private void Start()
    {
        if (GaugeManager.Instance == null) return;

        GaugeManager.Instance.OnGaugeChanged += OnGaugeChanged;

        // 初期値反映
        var gauge = GaugeManager.Instance.Gauges
            .Find(g => g.type == gaugeType);

        if (gauge != null)
        {
            OnGaugeChanged(gauge.type, gauge.current);
        }
    }

    private void OnDestroy()
    {
        if (GaugeManager.Instance != null)
        {
            GaugeManager.Instance.OnGaugeChanged -= OnGaugeChanged;
        }
    }

    private void OnGaugeChanged(GaugeType type, int value)
    {
        if (type != gaugeType) return;

        int targetCount = value / 10;

        // 増える場合
        while (spawnedObjects.Count < targetCount)
        {
            SpawnOne();
        }

        // 減る場合
        while (spawnedObjects.Count > targetCount)
        {
            RemoveOne();
        }
    }

    private void SpawnOne()
    {
        Vector3 pos =
            spawnCenter.position +
            Random.insideUnitSphere * radius;

        pos.y = spawnCenter.position.y;

        var obj = Instantiate(spawnPrefab, pos, Quaternion.identity);
        spawnedObjects.Add(obj);
    }

    private void RemoveOne()
    {
        int lastIndex = spawnedObjects.Count - 1;
        var obj = spawnedObjects[lastIndex];

        spawnedObjects.RemoveAt(lastIndex);
        Destroy(obj);
    }
}
