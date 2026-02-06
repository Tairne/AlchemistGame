using TMPro;
using UnityEngine;

public class TooltipUI : MonoBehaviour
{
    public static TooltipUI Instance { get; private set; }

    [SerializeField] private RectTransform tooltipRoot;
    [SerializeField] private TMP_Text tooltipText;

    [Header("Where tooltip is positioned (set to InventoryPanel RectTransform)")]
    [SerializeField] private RectTransform referenceArea;

    [SerializeField] private Vector2 offset = new Vector2(0f, 0f);

    Canvas _canvas;
    Camera _uiCamera;

    void Awake()
    {
        Instance = this;

        if (tooltipRoot == null)
            tooltipRoot = (RectTransform)transform;

        _canvas = GetComponentInParent<Canvas>();
        _uiCamera = (_canvas.renderMode == RenderMode.ScreenSpaceOverlay) ? null : _canvas.worldCamera;

        Hide();
    }

    public void Show(string text, RectTransform anchor)
    {
        if (tooltipText != null)
            tooltipText.text = text ?? "";

        tooltipRoot.gameObject.SetActive(true);

        if (anchor != null)
            PlaceNear(anchor);
    }

    public void Hide()
    {
        tooltipRoot.gameObject.SetActive(false);
    }

    void PlaceNear(RectTransform anchor)
    {
        if (referenceArea == null)
        {
            Debug.LogError("TooltipUI: referenceArea is not assigned (set InventoryPanel RectTransform).", this);
            return;
        }

        // Берём верхний правый угол кнопки в мире
        var corners = new Vector3[4];
        anchor.GetWorldCorners(corners);
        Vector3 worldPoint = corners[2]; // top-right

        // Переводим в экранные координаты
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(_uiCamera, worldPoint);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            referenceArea,
            screenPoint,
            _uiCamera,
            out Vector2 localPoint
        );

        tooltipRoot.SetParent(referenceArea, worldPositionStays: false);
        tooltipRoot.anchoredPosition = localPoint + offset;
    }
}
