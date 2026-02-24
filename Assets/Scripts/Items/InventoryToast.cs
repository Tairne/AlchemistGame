using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class InventoryToast : MonoBehaviour
{
    [SerializeField] private TMP_Text logText;
    [SerializeField] private float showSeconds = 2f;

    private readonly List<string> _titles = new();
    private Coroutine _hideRoutine;

    void Start()
    {
        if (InventoryManager.Instance == null)
        {
            Debug.LogError("InventoryToast: InventoryManager.Instance is null");
            enabled = false;
            return;
        }

        InventoryManager.Instance.OnItemAdded += OnAdded;

        // на всякий случай
        if (logText != null)
        {
            logText.text = "";
            logText.gameObject.SetActive(false);
        }
    }

    void OnDestroy()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnItemAdded -= OnAdded;
    }

    private void OnAdded(ItemData item)
    {
        if (item == null || logText == null) return;

        _titles.Add(item.title);

        // Собираем: "Получено: A, B, C"
        var sb = new StringBuilder("Получено: ");
        for (int i = 0; i < _titles.Count; i++)
        {
            if (i > 0) sb.Append(", ");
            sb.Append(_titles[i]);
        }

        logText.text = sb.ToString();
        logText.gameObject.SetActive(true);

        // Перезапускаем таймер скрытия
        if (_hideRoutine != null) StopCoroutine(_hideRoutine);
        _hideRoutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(showSeconds);

        _titles.Clear();
        logText.text = "";
        logText.gameObject.SetActive(false);
        _hideRoutine = null;
    }
}
