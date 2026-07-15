using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Deck : MonoBehaviour
{
    public GameObject[] cardPrefabs;
    public Transform controller;
    public Transform playerCamera;

    public int drawCost = 5;   // ★ カード1枚引くコスト

    void Awake()
    {
        var interactable =
            GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();

        interactable.selectEntered.AddListener(OnDeckClicked);
    }

    private void OnDeckClicked(SelectEnterEventArgs args)
    {
        // ★ Money が足りるかチェック
        if (!HasEnoughMoney())
        {
            Debug.Log("Money が足りません");
            return;
        }

        // ★ カード生成
        SpawnRandomCard();

        // ★ Money を消費
        GaugeManager.Instance.ChangeGauge(
            GaugeType.Money,
            -drawCost
        );
    }

    private bool HasEnoughMoney()
    {
        var moneyGauge = GaugeManager.Instance.Gauges
            .Find(g => g.type == GaugeType.Money);

        if (moneyGauge == null) return false;

        return moneyGauge.current >= drawCost;
    }

    private void SpawnRandomCard()
    {
        int index = Random.Range(0, cardPrefabs.Length);
        GameObject prefab = cardPrefabs[index];

        var card = Instantiate(prefab, transform.position, Quaternion.identity);

        var follow = card.GetComponent<CardFollowAndGrab>();
        follow.controller = controller;
        follow.playerCamera = playerCamera;
    }
}
