using UnityEngine;
using TMPro;

public class TurnUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI turnText;

    [Tooltip("イベント表示用の専用Text（未設定の場合は turnText に自動併記されます）")]
    public TextMeshProUGUI eventText;

    void Update()
    {
        if (TurnManager.Instance == null) return;

        string turnStr = $"Turn {TurnManager.Instance.CurrentTurn} / {TurnManager.Instance.MaxTurns}";

        if (turnText != null)
        {
            if (eventText == null)
            {
                // 専用テキストが無い場合は、turnTextの下行にイベント状態をスマートに併記
                if (!string.IsNullOrEmpty(TurnManager.Instance.LastEventMessage))
                {
                    turnText.text = $"{turnStr}\n<size=65%>{TurnManager.Instance.LastEventMessage}</size>";
                }
                else
                {
                    turnText.text = turnStr;
                }
            }
            else
            {
                turnText.text = turnStr;
            }
        }

        if (eventText != null)
        {
            eventText.text = TurnManager.Instance.LastEventMessage;
        }
    }
}
