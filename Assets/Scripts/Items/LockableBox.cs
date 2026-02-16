using UnityEngine;

public class LockableBox : MonoBehaviour, IInteractHint
{
    [Header("Key requirement")]
    [SerializeField] private ItemData requiredKey;
    [SerializeField] private bool consumeKey = false;

    [Header("Open behaviour")]
    [SerializeField] private GameObject closedVisual; // optional: модель/крышка закрытого состояния
    [SerializeField] private GameObject openedVisual; // optional: модель/крышка открытого состояния
    [SerializeField] private Animator animator;       // optional
    [SerializeField] private string openTrigger = "Open";

    public InteractAction GetAction()
    {
        if (InventoryManager.Instance == null)
            return InteractAction.None;

        return InventoryManager.Instance.Inventory.Contains(requiredKey)
            ? InteractAction.Open
            : InteractAction.NeedKey;
    }

    public void TryOpen()
    {
        if (!CompareTag("Lock"))
            return;

        if (requiredKey == null)
        {
            Debug.LogError($"LockableBox '{name}': requiredKey is not assigned", this);
            return;
        }

        var invMgr = InventoryManager.Instance;
        if (invMgr == null)
        {
            Debug.LogError("InventoryManager not found in scene");
            return;
        }

        if (!invMgr.Inventory.Contains(requiredKey))
        {
            Debug.Log($"'{name}' is locked. Need key: {requiredKey.id}");
            return;
        }

        if (consumeKey)
            invMgr.Remove(requiredKey);

        Open();
    }

    void Open()
    {
        // меняем тег на Box
        gameObject.tag = "Box";

        // визуал / анимация
        if (animator != null)
            animator.SetTrigger(openTrigger);

        if (closedVisual != null) closedVisual.SetActive(false);
        if (openedVisual != null) openedVisual.SetActive(true);
    }
}
