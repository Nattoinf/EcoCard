using UnityEngine;
using TMPro;

public class TurnUI : MonoBehaviour
{
    public TextMeshProUGUI turnText;

    void Update()
    {
        if (TurnManager.Instance == null) return;

        turnText.text =
            $"Turn {TurnManager.Instance.currentTurn} / {TurnManager.Instance.maxTurns}";
    }
}
