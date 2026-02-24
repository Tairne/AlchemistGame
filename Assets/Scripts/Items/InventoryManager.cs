using TMPro;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [SerializeField] private Inventory inventory = new();
    [SerializeField] private TMP_Text _logText;

    public Inventory Inventory => inventory;

    public ItemData SelectedItem { get; private set; }

    public event System.Action<ItemData> OnSelectedChanged;
    public event System.Action<ItemData> OnItemAdded;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public bool Add(ItemData item)
    {
        var added = inventory.Add(item);
        if (added)
            OnItemAdded?.Invoke(item);

        return added;
    }

    public bool Remove(ItemData item)
    {
        var removed = inventory.Remove(item);
        if (removed && SelectedItem == item)
            Select(null);

        return removed;
    }

    public void Select(ItemData item)
    {
        if (item != null && !inventory.Contains(item))
            return;

        SelectedItem = item;
        OnSelectedChanged?.Invoke(SelectedItem);
    }

    public TMP_Text LogText => _logText;
}
