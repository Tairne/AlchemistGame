using UnityEngine;
using UnityEngine.UI;

public class CraftStationController : MonoBehaviour, IInteractHint
{
    [SerializeField] private Recipe recipe;

    [Header("UI")]
    [SerializeField] private GameObject panel;
    [SerializeField] private Button craftButton;

    [Header("Required icons (hand-placed)")]
    [SerializeField] private Image[] inputIcons;

    [Header("World Effects (optional)")]
    [SerializeField] private GameObject disableOnCraft;
    [SerializeField] private GameObject enableOnCraft;
    [SerializeField] private bool disableSelfOnCraft = false;

    public InteractAction GetAction() => InteractAction.Use;

    void Start()
    {
        if (panel != null) panel.SetActive(false);
    }

    public void OpenPanel()
    {
        if (panel == null || craftButton == null || recipe == null)
        {
            Debug.LogError($"{name}: Missing UI or recipe", this);
            return;
        }

        RefreshUI();
        if (!UIWindowsManager.Instance.Open(panel))
            return;
    }

    private void RefreshUI()
    {
        var inv = InventoryManager.Instance.Inventory;

        bool canCraft = true;
        for (int i = 0; i < recipe.inputs.Count; i++)
        {
            if (!inv.Contains(recipe.inputs[i]))
            {
                canCraft = false;
                break;
            }
        }

        craftButton.interactable = canCraft;

        // заполняем иконки по порядку
        for (int i = 0; i < inputIcons.Length; i++)
        {
            var img = inputIcons[i];
            if (img == null) continue;

            if (i >= recipe.inputs.Count || recipe.inputs[i] == null)
            {
                img.enabled = false;          
                continue;
            }

            var item = recipe.inputs[i];
            bool has = inv.Contains(item);

            img.sprite = item.icon;
            img.enabled = has;                 
        }
    }

    public void Craft()
    {
        var inv = InventoryManager.Instance;

        foreach (var item in recipe.inputs)
            if (!inv.Inventory.Contains(item))
                return;

        foreach (var item in recipe.inputs)
            inv.Remove(item);

        foreach (var item in recipe.outputs)
            inv.Add(item);

        if (disableOnCraft != null) disableOnCraft.SetActive(false);
        if (disableSelfOnCraft) gameObject.SetActive(false);
        if (enableOnCraft != null) enableOnCraft.SetActive(true);

        RefreshUI();
    }

    public void Close()
    {
        panel.SetActive(false);
        UIWindowsManager.Instance.CloseAll();
    }
}
