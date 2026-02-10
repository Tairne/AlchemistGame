using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LootRowView : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text title;

    public void Bind(ItemData item)
    {
        if (title != null) title.text = item != null ? item.title : "";

        if (icon != null)
        {
            icon.sprite = item != null ? item.icon : null;
            icon.enabled = (item != null && item.icon != null);
        }
    }
}
