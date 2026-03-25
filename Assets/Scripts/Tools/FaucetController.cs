using UnityEngine;

public class FaucetController : MonoBehaviour, IInteractHint
{
    [SerializeField] private ItemData glass;
    [SerializeField] private ItemData water;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip waterSound;

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
            audioSource.PlayOneShot(waterSound);
            invMgr.Add(water);
            invMgr.Remove(glass);
        }
        else
        {
            audioSource.PlayOneShot(waterSound);
        }
    }
}
