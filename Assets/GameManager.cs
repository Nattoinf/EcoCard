using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Result UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject gameClearPanel;

    private bool gameEnded = false;

    public bool IsGameEnded => gameEnded;

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

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (gameClearPanel != null)
            gameClearPanel.SetActive(false);
    }

    public void CheckGameState()
    {
        if (gameEnded)
            return;

        // ゲージが1つでも0以下ならゲームオーバー
        if (GaugeManager.Instance != null)
        {
            foreach (var gauge in GaugeManager.Instance.Gauges)
            {
                if (gauge.current <= 0)
                {
                    GameOver(gauge.type);
                    return;
                }
            }
        }

        // 最大ターンまで到達し、ゲージが残っていればゲームクリア
        if (TurnManager.Instance != null &&
            TurnManager.Instance.CurrentTurn >=
            TurnManager.Instance.MaxTurns)
        {
            GameClear();
        }
    }

    private void GameOver(GaugeType depletedGauge)
    {
        gameEnded = true;

        Debug.Log($"GAME OVER: {depletedGauge} が0になりました");

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    private void GameClear()
    {
        gameEnded = true;

        Debug.Log("GAME CLEAR: 最大ターンまで生存しました");

        if (gameClearPanel != null)
            gameClearPanel.SetActive(true);
    }
}
