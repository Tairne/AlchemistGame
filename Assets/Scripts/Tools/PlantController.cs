using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlantController : MonoBehaviour, IInteractHint
{
    public static PlantController Instance { get; private set; }
    [SerializeField] private GameObject greenPlant;
    [SerializeField] private GameObject panel;
    [SerializeField] private Button waterButton;
    [SerializeField] private Image waterImage;
    [SerializeField] private ItemData water;

    public InteractAction GetAction() => InteractAction.Use;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (panel != null) panel.SetActive(false);
    }

    public void TryWater()
    {
        if (panel == null)
        {
            Debug.LogError("PlantController: UI refs are not assigned", this);
            return;
        }

        var invMgr = InventoryManager.Instance;

        if (!invMgr.Inventory.Contains(water))
        {
            waterButton.interactable = false;
        }
        else
        {
            waterImage.sprite = water.icon;
            waterButton.interactable = true;
        }

        var opened = UIWindowsManager.Instance.Open(panel);
        if (!opened)
            return;
    }

    public void Water()
    {
        var invMgr = InventoryManager.Instance;

        if (!invMgr.Inventory.Contains(water))
            return;

        gameObject.SetActive(false);
        greenPlant.SetActive(true);
    }

    public void ClosePanel()
    {
        panel.SetActive(false);
        UIWindowsManager.Instance.CloseAll();
    }
}
