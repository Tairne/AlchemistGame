using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItemButtonView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image icon;

    ItemData _item;
    System.Action<ItemData> _onClick;
    RectTransform _rect;

    void Awake()
    {
        _rect = (RectTransform)transform;
    }

    public void Bind(ItemData item, System.Action<ItemData> onClick)
    {
        _item = item;
        _onClick = onClick;

        if (icon != null)
        {
            icon.sprite = item != null ? item.icon : null;
            icon.enabled = (item != null && item.icon != null);
        }
    }

    public void Click()
    {
        if (_item != null)
            _onClick?.Invoke(_item);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_item != null && TooltipUI.Instance != null)
            TooltipUI.Instance.Show(_item.title, _rect);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipUI.Instance?.Hide();
    }
}
