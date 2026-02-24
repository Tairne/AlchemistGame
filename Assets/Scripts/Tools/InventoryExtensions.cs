using System.Collections.Generic;
using UnityEngine;

public static class InventoryExtensions
{
    public static bool Has(this List<ItemData> inv, ItemData item)
    {
        return item != null && inv.Contains(item);
    }

    public static bool Consume(this List<ItemData> inv, ItemData item)
    {
        if (!inv.Contains(item)) return false;
        inv.Remove(item);
        return true;
    }
}
