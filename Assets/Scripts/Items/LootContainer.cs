using System.Collections.Generic;
using UnityEngine;

public class LootContainer : MonoBehaviour
{
    [Header("What is inside")]
    public List<ItemData> items = new();

    [Header("One-time loot")]
    public bool oneTime = true;

    public bool IsEmpty => items == null || items.Count == 0;

    public List<ItemData> GetItems() => items;

    public void Clear()
    {
        items.Clear();
    }
}
