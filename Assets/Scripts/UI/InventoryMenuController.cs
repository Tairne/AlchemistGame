using StarterAssets;
using UnityEngine;

public class InventoryMenuController : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private bool pauseGame = true;

    [SerializeField] private FirstPersonController fpsController;
    [SerializeField] private GameObject centerPoint;
    [SerializeField] private InventoryUI inventoryUI;

    void Start()
    {
        // на старте меню закрыто
        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);

        ApplyState(isOpen: false);
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
        inventoryPanel.SetActive(open);

        ApplyState(open);

        // когда открыли: обновим список и покажем детали выбранного
        if (open && inventoryUI != null && InventoryManager.Instance != null)
        {
            inventoryUI.Rebuild();
            inventoryUI.ShowDetails(InventoryManager.Instance.SelectedItem);
        }
    }

    void ApplyState(bool isOpen)
    {
        Cursor.visible = isOpen;
        Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;

        if (fpsController != null)
            fpsController.SetControlsEnabled(!isOpen);

        if (centerPoint != null)
            centerPoint.SetActive(!isOpen);

        if (pauseGame)
            Time.timeScale = isOpen ? 0f : 1f;
    }

    void OnDisable()
    {
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (fpsController != null)
            fpsController.SetControlsEnabled(true);

        if (centerPoint != null)
            centerPoint.SetActive(true);
    }
}
