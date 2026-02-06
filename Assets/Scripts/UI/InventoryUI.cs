using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject inventoryPanel;

    [Header("List")]
    [SerializeField] private Transform itemsContainer;
    [SerializeField] private InventoryItemButtonView itemButtonPrefab;

    [Header("Details")]
    [SerializeField] private Image detailsIcon;
    [SerializeField] private TMP_Text detailsTitle;
    [SerializeField] private TMP_Text detailsDescription;

    void OnEnable()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.Inventory.OnChanged += Rebuild;
    }

    void OnDisable()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.Inventory.OnChanged -= Rebuild;
    }

    void Start()
    {
        Rebuild();
        ShowDetails(null);
    }

    public void Rebuild()
    {
        if (itemsContainer == null || itemButtonPrefab == null) return;
        if (InventoryManager.Instance == null) return;

        // очистить старые кнопки
        for (int i = itemsContainer.childCount - 1; i >= 0; i--)
            Destroy(itemsContainer.GetChild(i).gameObject);

        // создать новые
        var inv = InventoryManager.Instance.Inventory;
        foreach (var item in inv.Items)
        {
            var btn = Instantiate(itemButtonPrefab, itemsContainer);
            btn.Bind(item, OnItemClicked);
        }
    }

    void OnItemClicked(ItemData item)
    {
        InventoryManager.Instance.Select(item);
        ShowDetails(item);
    }

    public void ShowDetails(ItemData item)
    {
        if (detailsTitle != null) detailsTitle.text = item != null ? item.title : "";
        if (detailsDescription != null) detailsDescription.text = item != null ? item.description : "";

        if (detailsIcon != null)
        {
            detailsIcon.sprite = item != null ? item.icon : null;
            detailsIcon.enabled = (item != null && item.icon != null);
        }
    }
}
