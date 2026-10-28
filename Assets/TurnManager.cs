using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    [Header("Turn Settings")]
    [SerializeField] private int maxTurns = 10;
    [SerializeField] private int currentTurn = 1;

    // GameManagerなどから読み取るためのプロパティ
    public int CurrentTurn => currentTurn;
    public int MaxTurns => maxTurns;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // カードが1枚使われたときに呼ぶ
    public void UseCard()
    {
        // すでにゲーム終了済みならターンを進めない
        if (GameManager.Instance != null &&
            GameManager.Instance.IsGameEnded)
        {
            return;
        }

        Debug.Log($"Turn {currentTurn}/{maxTurns} Card Played");

        // 最大ターン（10ターン目）のカードをプレイし終えたらゲームクリア判定
        if (currentTurn >= maxTurns)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.CheckGameClear();
            }
            return;
        }

        currentTurn++;
    }
}
