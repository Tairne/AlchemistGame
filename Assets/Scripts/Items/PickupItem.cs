using UnityEngine;

public class PickupItem : MonoBehaviour, IInteractHint
{
    [SerializeField] private AudioClip pickSound;
    [SerializeField] private AudioSource pickSoundSource;

    public ItemData item;

    public InteractAction GetAction() => InteractAction.Take;

    public void Pickup()
    {
        if (item == null)
        {
            Debug.LogError("PickupItem: item is not assigned", this);
            return;
        }

        if (InventoryManager.Instance == null)
        {
            Debug.LogError("InventoryManager not found in scene");
            return;
        }

        var added = InventoryManager.Instance.Add(item);
        if (!added)
        {
            Debug.Log($"Item already in inventory: {item.id}");
            return;
        }

        pickSoundSource.PlayOneShot(pickSound);
        Debug.Log($"Added to inventory: {item.id} ({item.title})");
        gameObject.SetActive(false);
    }
}
