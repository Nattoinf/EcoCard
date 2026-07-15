using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public int maxTurns = 10;
    public int currentTurn = 0;

    public static TurnManager Instance;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // ★ カードが1枚使われたときに呼ぶ
    public void UseCard()
    {
        currentTurn++;
        Debug.Log($"Turn {currentTurn}/{maxTurns}");

        if (currentTurn >= maxTurns)
        {
            EndGame();
        }
    }

    void EndGame()
    {
        Debug.Log("10ターン終了！ゲーム終了");

        // ここに終了処理を書く
        // 例：
        // Time.timeScale = 0;
        // リザルト表示
        // シーン遷移
    }
}
