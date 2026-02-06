using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance { get; private set; }

    [Tooltip("Перетащи сюда ВСЕ ItemData")]
    public List<ItemData> allItems = new();

    Dictionary<string, ItemData> _byId;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        BuildIndex();
    }

    void BuildIndex()
    {
        _byId = new Dictionary<string, ItemData>();

        foreach (var item in allItems)
        {
            if (item == null) continue;

            if (string.IsNullOrWhiteSpace(item.id))
            {
                Debug.LogError($"ItemData '{item.name}' has empty id", item);
                continue;
            }

            if (_byId.ContainsKey(item.id))
            {
                Debug.LogError($"Duplicate item id '{item.id}'. Second: {item.name}", item);
                continue;
            }

            _byId.Add(item.id, item);
        }
    }

    public ItemData GetById(string id)
    {
        if (_byId != null && _byId.TryGetValue(id, out var item))
            return item;

        Debug.LogError($"Item id not found: {id}");
        return null;
    }
}
