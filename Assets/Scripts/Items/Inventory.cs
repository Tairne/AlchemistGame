using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Inventory
{
    [SerializeField] private List<ItemData> items = new();

    public IReadOnlyList<ItemData> Items => items;

    public event Action OnChanged;

    public bool Contains(ItemData item)
    {
        return item != null && items.Contains(item);
    }

    public bool Add(ItemData item)
    {
        if (item == null) return false;

        // Для квеста чаще всего предмет уникальный: либо есть, либо нет
        if (items.Contains(item))
            return false;

        items.Add(item);
        OnChanged?.Invoke();
        return true;
    }

    public bool Remove(ItemData item)
    {
        if (item == null) return false;

        var removed = items.Remove(item);
        if (removed) OnChanged?.Invoke();
        return removed;
    }

    public void Clear()
    {
        items.Clear();
        OnChanged?.Invoke();
    }
}
