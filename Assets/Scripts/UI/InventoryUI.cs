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
    [SerializeField] private Button readButton;

    private ItemData _selectedItem;

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
        if (readButton != null)
            readButton.onClick.AddListener(OnReadClicked);

        Rebuild();
        ShowDetails(null);
    }

    public void Rebuild()
    {
        if (itemsContainer == null || itemButtonPrefab == null) return;
        if (InventoryManager.Instance == null) return;

        for (int i = itemsContainer.childCount - 1; i >= 0; i--)
            Destroy(itemsContainer.GetChild(i).gameObject);

        var inv = InventoryManager.Instance.Inventory;
        foreach (var item in inv.Items)
        {
            var btn = Instantiate(itemButtonPrefab, itemsContainer);
            btn.Bind(item, OnItemClicked);
        }
    }

    void OnItemClicked(ItemData item)
    {
        _selectedItem = item;
        InventoryManager.Instance.Select(item);
        ShowDetails(item);
    }

    public void ShowDetails(ItemData item)
    {
        _selectedItem = item;

        if (detailsTitle != null)
            detailsTitle.text = item != null ? item.title : "";

        if (detailsDescription != null)
            detailsDescription.text = item != null ? item.description : "";

        if (detailsIcon != null)
        {
            detailsIcon.sprite = item != null ? item.icon : null;
            detailsIcon.enabled = (item != null && item.icon != null);
        }

        if (readButton != null)
        {
            bool canShowRead =
                item != null &&
                item.canRead &&
                item.note != null;

            readButton.gameObject.SetActive(canShowRead);
        }
    }

    private void OnReadClicked()
    {
        if (_selectedItem == null)
            return;

        if (!_selectedItem.canRead || _selectedItem.note == null)
            return;

        if (NoteReaderUI.Instance == null)
        {
            Debug.LogWarning("NoteReaderUI.Instance is null");
            return;
        }

        NoteReaderUI.Instance.Open(_selectedItem.note);
    }
}