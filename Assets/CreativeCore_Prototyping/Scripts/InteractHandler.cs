using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Cinemachine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class InteractHandler : MonoBehaviour
{
    public GameObject UIPrefab;

    public Sprite ItemPointer;
    public Sprite BoxPointer;
    public Sprite LockPointer;
    public Sprite ToolPointer;
    public Sprite DefaultInteractPointer;
    public Sprite NormalPointer;

    Image m_PointerImage;
    private Vector3 m_OriginalPointerSize;

    // NEW: hint
    [Header("Hint UI (inside UIPrefab)")]
    [SerializeField] private TMP_Text hintText;
    [SerializeField] private GameObject hintRoot;

    void Start()
    {
        Application.targetFrameRate = 60;

        var uiInstance = Instantiate(UIPrefab);

        if (hintText == null)
            hintText = uiInstance.GetComponentInChildren<TMP_Text>(true); 
        if (hintRoot == null && hintText != null)
            hintRoot = hintText.transform.parent.gameObject;

        HideHint();

        var mainCam = Camera.main;
        var cinemachineBrain = mainCam.GetComponent<CinemachineBrain>();
        if (cinemachineBrain == null)
            mainCam.gameObject.AddComponent<CinemachineBrain>();

        var centerPoint = GameObject.Find("CenterPoint");
        if (centerPoint != null)
        {
            m_PointerImage = centerPoint.GetComponent<Image>();
            m_OriginalPointerSize = centerPoint.transform.localScale;
        }
    }

    void Update()
    {
        if (UIWindowsManager.Instance != null && UIWindowsManager.Instance.IsAnyWindowOpen)
        {
            HideHint();
            return;
        }

#if ENABLE_INPUT_SYSTEM
        //bool interactPressed = Mouse.current.leftButton.wasPressedThisFrame || Keyboard.current.eKey.wasPressedThisFrame;
        bool interactPressed = Keyboard.current.eKey.wasPressedThisFrame;
#else
        //bool interactPressed = Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E);
        bool interactPressed = Input.GetKeyDown(KeyCode.E);
#endif

        OnInteract[] targets = null;
        RaycastHit hit;
        bool displayInteractable = false;

        var ray = Camera.main.ViewportPointToRay(Vector3.one * 0.5f);

        if (Physics.Raycast(ray, out hit, 2.0f))
        {
            var go = hit.collider.gameObject;

            var interacts = go.GetComponentsInChildren<OnInteract>();

            if (interacts.Length > 0)
            {
                displayInteractable = true;
                targets = interacts;

                if (m_PointerImage != null)
                {
                    m_PointerImage.color = Color.white;

                    foreach (var target in targets)
                    {
                        if (!target.isActiveAndEnabled)
                        {
                            m_PointerImage.color = Color.grey;
                            break;
                        }
                    }
                }

                // NEW: показать подсказку (берём label с объекта или родителя)
                ShowHintFor(go);
            }
            else
            {
                HideHint();
            }

            // Иконка по тегу
            if (displayInteractable && m_PointerImage != null)
            {
                Sprite chosenPointer = DefaultInteractPointer;

                if (go.CompareTag("Item"))
                    chosenPointer = ItemPointer;
                else if (go.CompareTag("Lock"))
                    chosenPointer = LockPointer;
                else if (go.CompareTag("Tool"))
                    chosenPointer = ToolPointer;
                else if (go.CompareTag("Box"))
                    chosenPointer = BoxPointer;

                m_PointerImage.sprite = chosenPointer;
                m_PointerImage.transform.localScale = m_OriginalPointerSize * 3.0f;
            }
        }
        else
        {
            HideHint();
        }

        if (targets != null && interactPressed)
        {
            foreach (var target in targets)
            {
                if (target.isActiveAndEnabled)
                    target.Interact();
            }
        }

        if (!displayInteractable && m_PointerImage != null)
        {
            m_PointerImage.sprite = NormalPointer;
            m_PointerImage.color = Color.white;
            m_PointerImage.transform.localScale = m_OriginalPointerSize;
        }
    }

    void ShowHintFor(GameObject hitObject)
    {
        if (hintText == null) return;

        // Ищем имя на объекте или выше
        var label = hitObject.GetComponentInParent<InteractableLabel>();

        string text;
        if (label != null && !string.IsNullOrWhiteSpace(label.DisplayName))
        {
            text = label.DisplayName;
        }
        else
        {
            // запасной вариант: красиво из тега
            if (hitObject.CompareTag("Lock")) text = "Заперто";
            else if (hitObject.CompareTag("Box")) text = "Коробка";
            else if (hitObject.CompareTag("Item")) text = "Предмет";
            else text = hitObject.name; // крайний fallback, лучше не полагаться
        }

        hintText.text = text;

        if (hintRoot != null && !hintRoot.activeSelf)
            hintRoot.SetActive(true);
    }

    void HideHint()
    {
        if (hintRoot != null && hintRoot.activeSelf)
            hintRoot.SetActive(false);
    }
}
