using System.Collections.Generic;
using UnityEngine;

public class LootContainer : MonoBehaviour, IInteractHint
{
    [Header("What is inside")]
    public List<ItemData> items = new();

    [Header("One-time loot")]
    public bool oneTime = true;

    private bool opened;

    public bool IsEmpty => items == null || items.Count == 0;

    public InteractAction GetAction()
    {
        if (opened && oneTime)
            return InteractAction.Empty;

        return InteractAction.Open;
    }

    public List<ItemData> GetItems()
    {
        return items;
    }

    public void Clear()
    {
        items.Clear();
    }

    public void MarkOpened()
    {
        opened = true;
    }
}
