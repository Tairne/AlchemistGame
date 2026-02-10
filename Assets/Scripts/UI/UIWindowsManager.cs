using UnityEngine;
using StarterAssets;

public class UIWindowsManager : MonoBehaviour
{
    public static UIWindowsManager Instance { get; private set; }

    [SerializeField] private GameObject[] windows;
    [SerializeField] private bool pauseGame = true;
    [SerializeField] private FirstPersonController fpsController;
    [SerializeField] private GameObject centerPoint;
    [Header("Blocking")]
    [SerializeField] private GameObject inventoryPanel;

    GameObject _openWindow;
    bool _blockedByInventory;

    void Awake() => Instance = this;

    public bool IsInventoryOpen => inventoryPanel != null && inventoryPanel.activeSelf;
    public bool IsAnyWindowOpen { get; private set; }
    public void CloseAll()
    {
        CloseAllInternal();
        _openWindow = null;
        IsAnyWindowOpen = false;
        ApplyState(false);
    }

    public bool Open(GameObject window)
    {
        if (window == null) return false;

        if (IsInventoryOpen && window != inventoryPanel)
            return false;

        CloseAllInternal();

        window.SetActive(true);
        _openWindow = window;
        IsAnyWindowOpen = true;

        ApplyState(true);
        return true;
    }

    void CloseAllInternal()
    {
        foreach (var w in windows)
            if (w != null) w.SetActive(false);
    }

    void ApplyState(bool anyWindowOpen)
    {
        Cursor.visible = anyWindowOpen;
        Cursor.lockState = anyWindowOpen ? CursorLockMode.None : CursorLockMode.Locked;

        fpsController?.SetControlsEnabled(!anyWindowOpen);
        if (centerPoint != null) centerPoint.SetActive(!anyWindowOpen);

        if (pauseGame)
            Time.timeScale = anyWindowOpen ? 0f : 1f;
    }
}

