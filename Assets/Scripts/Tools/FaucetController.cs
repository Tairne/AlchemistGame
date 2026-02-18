using UnityEngine;

public class FaucetController : MonoBehaviour, IInteractHint
{
    [SerializeField] private ItemData glass;
    [SerializeField] private ItemData water;

    public InteractAction GetAction() => InteractAction.Use;

    public void Use()
    {
        var invMgr = InventoryManager.Instance;
        if (invMgr == null)
        {
            Debug.LogError("InventoryManager not found in scene");
            return;
        }

        if (invMgr.Inventory.Contains(glass))
        {
            Debug.Log("Налили водички");
            invMgr.Inventory.Add(water);
            invMgr.Inventory.Remove(glass);
        }
        else
        {
            Debug.Log("Водички не налили, но пошуршали");
        }
    }
}
