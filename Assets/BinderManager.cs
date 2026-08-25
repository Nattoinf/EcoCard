using System.Collections.Generic;
using UnityEngine;

public class BinderManager : MonoBehaviour
{
    public static BinderManager Instance;

    public Transform[] slots;

    private readonly List<CardFollowAndGrab> hand = new();

    private void Awake()
    {
        Instance = this;
    }

    public bool AddCard(CardFollowAndGrab card)
    {
        if (hand.Contains(card))
            return true;

        if (hand.Count >= slots.Length)
            return false;

        hand.Add(card);

        card.SetBinder(this);

        Layout();

        return true;
    }

    public void RemoveCard(CardFollowAndGrab card)
    {
        if (hand.Remove(card))
        {
            Layout();
        }
    }

    public void Layout()
    {
        for (int i = 0; i < hand.Count; i++)
        {
            hand[i].transform.SetParent(slots[i], false);

            hand[i].transform.localPosition = Vector3.zero;
            hand[i].transform.localRotation = Quaternion.identity;

            hand[i].MoveToBinder();
        }
    }
}
