using StarterAssets;
using UnityEngine;

public class InventoryMenuController : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private InventoryUI inventoryUI;

    void Start()
    {
        // на старте меню закрыто
        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            Toggle();
    }

    public void Toggle()
    {
        if (inventoryPanel == null)
        {
            Debug.LogError("InventoryMenuController: inventoryPanel is NULL", this);
            return;
        }

        bool open = !inventoryPanel.activeSelf;

        if (open)
        {
            UIWindowsManager.Instance.Open(inventoryPanel);

            if (inventoryUI != null && InventoryManager.Instance != null)
            {
                inventoryUI.Rebuild();

                // ≈сли ничего не выбрано, выберем первый предмет, чтобы детали не были пустые
                var inv = InventoryManager.Instance.Inventory;
                if (InventoryManager.Instance.SelectedItem == null && inv.Items.Count > 0)
                    InventoryManager.Instance.Select(inv.Items[0]);

                inventoryUI.ShowDetails(InventoryManager.Instance.SelectedItem);
            }
        }
        else
        {
            UIWindowsManager.Instance.CloseAll();
        }
    }
}
