using UnityEngine;

public class SlotManager : MonoBehaviour
{
    public static SlotManager Instance;

    private Slot[] slots;

    private void Awake()
    {
        Instance = this;
        slots = FindObjectsOfType<Slot>();
    }

    public void HighlightAvailableSlots(CardData card)
    {
        foreach (var slot in slots)
        {
            bool canUse = slot.CanAccept(card);
            slot.SetHighlight(canUse);
        }
    }

    public void ClearHighlight()
    {
        foreach (var slot in slots)
        {
            slot.SetHighlight(false);
        }
    }
}
