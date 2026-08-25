using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Deck : MonoBehaviour
{
    public GameObject[] cardPrefabs;

    public int drawCost = 5;

    void Awake()
    {
        GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>()
            .selectEntered.AddListener(OnDeckClicked);
    }

    void OnDeckClicked(SelectEnterEventArgs args)
    {
        if (!HasEnoughMoney())
            return;

        SpawnRandomCard();

        GaugeManager.Instance.ChangeGauge(
            GaugeType.Money,
            -drawCost
        );
    }

    void SpawnRandomCard()
    {
        int index = Random.Range(0, cardPrefabs.Length);

        GameObject card = Instantiate(cardPrefabs[index]);

        BinderManager.Instance.AddCard(
            card.GetComponent<CardFollowAndGrab>()
        );
    }

    bool HasEnoughMoney()
    {
        var moneyGauge = GaugeManager.Instance.Gauges
            .Find(g => g.type == GaugeType.Money);

        return moneyGauge != null &&
               moneyGauge.current >= drawCost;
    }
}
