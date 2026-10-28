using UnityEngine;
using TMPro;

public class TurnUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI turnText;

    [Tooltip("イベント表示用の専用Text（未設定の場合は turnText に自動併記されます）")]
    public TextMeshProUGUI eventText;

    private void Awake()
    {
        // 2行表示になった際に文字が枠外でクリップされないように設定を保証
        if (turnText != null)
        {
            turnText.enableWordWrapping = false;
            turnText.overflowMode = TextOverflowModes.Overflow;
        }
    }

    void Update()
    {
        if (TurnManager.Instance == null) return;

        string turnStr = $"Turn {TurnManager.Instance.CurrentTurn} / {TurnManager.Instance.MaxTurns}";

        if (turnText != null)
        {
            if (eventText == null)
            {
                // 専用テキストが無い場合は、turnTextの下行にイベント状態をカラー付きで併記
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
