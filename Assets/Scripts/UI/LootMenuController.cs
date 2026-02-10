using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class LootMenuController : MonoBehaviour
{
    public static LootMenuController Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject lootPanel;
    [SerializeField] private Transform itemsContainer;
    [SerializeField] private LootRowView lootRowPrefab;

    LootContainer _current;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (lootPanel != null) lootPanel.SetActive(false);
    }

    public void Open(LootContainer container)
    {
        if (container == null) return;

        _current = container;

        if (lootPanel == null || itemsContainer == null || lootRowPrefab == null)
        {
            Debug.LogError("LootMenuController: UI refs are not assigned", this);
            return;
        }

        Rebuild(container.GetItems());

        var opened = UIWindowsManager.Instance.Open(lootPanel);
        if (!opened)
            return;
    }

    public void Close()
    {
        lootPanel.SetActive(false);
        UIWindowsManager.Instance.CloseAll();

        _current = null;

        // очистим список в UI, чтобы не мигало старьё
        ClearRows();
    }

    public void TakeAll()
    {
        if (_current == null) { Close(); return; }
        if (InventoryManager.Instance == null) { Debug.LogError("InventoryManager missing"); return; }

        var inv = InventoryManager.Instance;

        // добавляем всё, что есть
        foreach (var item in _current.GetItems())
        {
            if (item == null) continue;
            inv.Add(item);
        }

        // очищаем контейнер
        if (_current.oneTime)
        {
            _current.Clear();
        }

        Close();
    }

    void Rebuild(List<ItemData> items)
    {
        ClearRows();

        if (items == null) return;

        foreach (var item in items)
        {
            var row = Instantiate(lootRowPrefab, itemsContainer);
            row.Bind(item);
        }
    }

    void ClearRows()
    {
        for (int i = itemsContainer.childCount - 1; i >= 0; i--)
            Destroy(itemsContainer.GetChild(i).gameObject);
    }
}
