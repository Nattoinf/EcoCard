using UnityEngine;

[System.Serializable]
public class GaugeEffect
{
    public GaugeType type;
    public int amount;
}

public class CardData : MonoBehaviour
{
    public GameObject spawnPrefab;
    public Vector3 spawnOffset;

    public GaugeEffect[] gaugeEffects;
}
