using UnityEngine;

public class LootOpener : MonoBehaviour
{
    [SerializeField] private LootContainer container;

    public void Open()
    {
        if (container == null)
            container = GetComponent<LootContainer>();

        if (!CompareTag("Box"))
            return;

        if (container == null)
        {
            Debug.LogError("LootOpener: LootContainer not found", this);
            return;
        }

        if (LootMenuController.Instance == null)
        {
            Debug.LogError("LootMenuController not found in scene");
            return;
        }

        LootMenuController.Instance.Open(container);
    }
}