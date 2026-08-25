using UnityEngine;

public class Slot : MonoBehaviour
{
    public SlotType slotType;

    [Header("Snap")]
    public Transform snapPoint;
    public Transform spawnPoint;
    public float snapDistance = 0.5f;

    [Header("Highlight")]
    public Renderer slotRenderer;
    public Color normalColor = Color.white;
    public Color highlightColor = Color.yellow;

    private bool isOccupied = false;

    private void Start()
    {
        SetHighlight(false);
    }

    
    // 使用可能判定
    

    public bool CanAccept(CardData data)
    {
        if (data == null) return false;

        bool isSpawnCard = data.spawnPrefab != null;
        bool isEffectCard = data.spawnPrefab == null;

        if (slotType == SlotType.Spawn && isSpawnCard)
            return true;

        if (slotType == SlotType.Effect && isEffectCard)
            return true;

        return false;
    }

    
    // Highlight
    

    public void SetHighlight(bool on)
    {
        if (slotRenderer == null)
            return;

        slotRenderer.material.color =
            on ? highlightColor : normalColor;
    }

    
    // Card Placement
    

    private void OnTriggerStay(Collider other)
    {
        if (isOccupied)
            return;

        if (!other.CompareTag("Card"))
            return;

        if (!other.TryGetComponent(out CardFollowAndGrab follow))
            return;

        // まだ掴まれているなら置かない
        //if (follow.IsGrabbed)
        //    return;

        if (!other.TryGetComponent(out CardData data))
            return;

        if (!CanAccept(data))
            return;

        if (Vector3.Distance(
            other.transform.position,
            snapPoint.position) > snapDistance)
            return;

        PlaceCard(other.gameObject, follow, data);
    }

    
    // 実際の配置
    

    private void PlaceCard(
        GameObject card,
        CardFollowAndGrab follow,
        CardData data)
    {
        if (follow.IsUsed){
            return;
        }
        follow.MarkUsed();
        if (slotType == SlotType.Spawn){
            isOccupied = true;
        }

        follow.EnterSlot();

        card.transform.SetParent(snapPoint);

        card.transform.localPosition = Vector3.zero;
        card.transform.localRotation = Quaternion.identity;
        card.transform.localScale = new Vector3(0.3f, 0.1f, 0.2f);

        Rigidbody rb = card.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        foreach (var effect in data.gaugeEffects)
        {
            GaugeManager.Instance.ChangeGauge(
                effect.type,
                effect.amount
            );
        }

        if (slotType == SlotType.Spawn &&
            data.spawnPrefab != null)
        {
            Instantiate(
                data.spawnPrefab,
                spawnPoint.position + data.spawnOffset,
                spawnPoint.rotation
            );
        }

        TurnManager.Instance.UseCard();

        if (slotType == SlotType.Spawn){
            Destroy(card);
        }
    }

    // 空きスロットへ戻す

    public void ClearSlot()
    {
        isOccupied = false;
    }
}
