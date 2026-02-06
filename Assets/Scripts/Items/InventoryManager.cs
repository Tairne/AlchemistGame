using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [SerializeField] private Inventory inventory = new();

    public Inventory Inventory => inventory;

    public ItemData SelectedItem { get; private set; }

    public event System.Action<ItemData> OnSelectedChanged;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public bool Add(ItemData item) => inventory.Add(item);

    public bool Remove(ItemData item)
    {
        // если удалили выбранный, сбросим выбор
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
}
