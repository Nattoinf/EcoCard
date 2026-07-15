using UnityEngine;

public class Slot : MonoBehaviour
{
    public SlotType slotType;

    [Header("Snap")]
    public Transform snapPoint;
    public Transform spawnPoint;
    public float snapDistance = 0.1f;

    [Header("Highlight")]
    public Renderer slotRenderer;
    public Color normalColor = Color.white;
    public Color highlightColor = Color.yellow;

    private bool isOccupied = false;

    private void Start()
    {
        SetHighlight(false);
    }

    // =========================
    // ハイライト制御
    // =========================
    public bool CanAccept(CardData data)
    {
        if (data == null) return false;

        bool isSpawnCard  = data.spawnPrefab != null;
        bool isEffectCard = data.spawnPrefab == null;

        if (slotType == SlotType.Spawn && isSpawnCard) return true;
        if (slotType == SlotType.Effect && isEffectCard) return true;

        return false;
    }

    public void SetHighlight(bool on)
    {
        if (slotRenderer == null) return;

        // マテリアル共有対策（重要）
        if (slotRenderer.material != null)
        {
            slotRenderer.material.color =
                on ? highlightColor : normalColor;
        }
    }

    // =========================
    // 吸着・使用処理
    // =========================
    private void OnTriggerStay(Collider other)
    {
        if (isOccupied && slotType == SlotType.Spawn) return;
        if (!other.CompareTag("Card")) return;

        if (Vector3.Distance(other.transform.position, snapPoint.position) > snapDistance)
            return;

        var data = other.GetComponent<CardData>();
        if (data == null)
        {
            Debug.LogWarning("CardData がありません");
            return;
        }

        // ★ スロット × カード の適合チェック
        if (!CanAccept(data)) return;

        // 追従停止
        if (other.TryGetComponent<CardFollowAndGrab>(out var follow))
            follow.EnterSlot();

        // 吸着
        other.transform.SetPositionAndRotation(
            snapPoint.position,
            snapPoint.rotation
        );

        // 物理停止
        if (other.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // ★ 効果発動
        foreach (var effect in data.gaugeEffects)
        {
            GaugeManager.Instance.ChangeGauge(effect.type, effect.amount);
        }

        // ★ Spawn スロットのみ生成
        if (slotType == SlotType.Spawn)
        {
            Vector3 spawnPos = spawnPoint.position + data.spawnOffset;
            Instantiate(data.spawnPrefab, spawnPos, spawnPoint.rotation);

            isOccupied = true;
        }

        // ★ ターン消費
        TurnManager.Instance.UseCard();

        // ★ カード消失
        Destroy(other.gameObject, 0.05f);
    }
}
