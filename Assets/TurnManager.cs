using System.Collections;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    [Header("Turn Settings")]
    [SerializeField] private int maxTurns = 10;
    [SerializeField] private int currentTurn = 1;

    [Header("Random Event Phase Settings")]
    [Range(0f, 1f)]
    [Tooltip("ランダムイベントの発生確率 (0.0 = 0%, 1.0 = 100%)")]
    [SerializeField] private float eventProbability = 0.5f;

    [Tooltip("イベント結果を画面に表示・維持する待機時間（秒）")]
    [SerializeField] private float eventDisplayDuration = 2.0f;

    private bool isEventPhase = false;
    private string lastEventMessage = "<color=#AAAAAA>[Waiting for Card]</color>";
    private bool isProcessingTurn = false;

    // プロパティ
    public int CurrentTurn => currentTurn;
    public int MaxTurns => maxTurns;
    public bool IsEventPhase => isEventPhase;
    public string LastEventMessage => lastEventMessage;
    public float EventProbability => eventProbability;

    // カードをプレイ可能な状態か（イベント処理中やゲーム終了時はプレイ不可）
    public bool CanPlayCard => !isProcessingTurn && !(GameManager.Instance != null && GameManager.Instance.IsGameEnded);

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        lastEventMessage = "<color=#AAAAAA>[Waiting for Card]</color>";
    }

    // カードが1枚使われたときに呼ぶ
    public void UseCard()
    {
        // すでにゲーム終了済みならターンを進めない
        if (GameManager.Instance != null && GameManager.Instance.IsGameEnded)
        {
            return;
        }

        // ターン処理中は重複呼び出しを防止
        if (isProcessingTurn)
        {
            return;
        }

        StartCoroutine(TurnSequenceCoroutine());
    }

    private IEnumerator TurnSequenceCoroutine()
    {
        isProcessingTurn = true;
        Debug.Log($"Turn {currentTurn}/{maxTurns} Card Played. Entering Random Event Phase...");

        // === 1. ランダムイベントフェーズ ===
        isEventPhase = true;

        // 確率によるイベント発生判定
        bool eventOccurred = Random.value < eventProbability;
        if (eventOccurred)
        {
            // Unity標準フォント(LiberationSans)でも文字化けしないよう英字＆カラータグで表示
            lastEventMessage = "<color=#FFCC00><b>★ EVENT OCCURRED!</b></color>";
            Debug.Log($"<color=yellow>[Event Phase] イベントが発生しました！ (Turn {currentTurn})</color>");
        }
        else
        {
            lastEventMessage = "<color=#88FF88><b>- No Event (Peaceful) -</b></color>";
            Debug.Log($"<color=cyan>[Event Phase] イベントは発生しませんでした。 (Turn {currentTurn})</color>");
        }

        // イベント結果の表示時間を待機
        yield return new WaitForSeconds(eventDisplayDuration);

        isEventPhase = false;

        // === 2. ターン終了・クリア判定 ===
        if (currentTurn >= maxTurns)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.CheckGameClear();
            }
            isProcessingTurn = false;
            yield break;
        }

        // 次のターンへ
        currentTurn++;
        Debug.Log($"Advanced to Turn {currentTurn}/{maxTurns}");

        isProcessingTurn = false;
    }
}
