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
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text actionText;
    [SerializeField] private GameObject hintRoot;

    void Start()
    {
        Application.targetFrameRate = 60;

        var canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("No Canvas found in scene. UI cannot be rendered.");
            return;
        }

        var uiInstance = Instantiate(UIPrefab, canvas.transform, false);

        // ---- Find UI refs строго по имени/пути ----
        var hintRootTr = uiInstance.transform.Find("InteractHint");
        hintRoot = hintRootTr != null ? hintRootTr.gameObject : null;

        var titleTr = uiInstance.transform.Find("InteractHint/TitleText");
        titleText = titleTr != null ? titleTr.GetComponent<TMP_Text>() : null;

        var actionTr = uiInstance.transform.Find("InteractHint/ActionText");
        actionText = actionTr != null ? actionTr.GetComponent<TMP_Text>() : null;

        // CenterPoint теперь тоже лучше искать внутри uiInstance, а не по всей сцене
        var centerPointTr = uiInstance.transform.Find("CenterPoint");
        if (centerPointTr != null)
        {
            m_PointerImage = centerPointTr.GetComponent<Image>();
            m_OriginalPointerSize = centerPointTr.localScale;
        }
        else
        {
            Debug.LogError("CenterPoint not found in UIPrefab (expected: InteractUI/CenterPoint)");
        }
        Debug.Log($"Pointer object: {(m_PointerImage != null ? m_PointerImage.gameObject.name : "NULL")}");
        // Проверка, чтобы не гадать потом "почему не работает"
        Debug.Log($"[InteractHandler] hintRoot={(hintRoot != null)} title={(titleText != null)} action={(actionText != null)} pointer={(m_PointerImage != null)}");

        HideHint();

        // Cinemachine brain (оставляем как было)
        var mainCam = Camera.main;
        if (mainCam != null)
        {
            var cinemachineBrain = mainCam.GetComponent<CinemachineBrain>();
            if (cinemachineBrain == null)
                mainCam.gameObject.AddComponent<CinemachineBrain>();
        }
    }

    void Update()
    {
        bool uiOpen = UIWindowsManager.Instance != null && UIWindowsManager.Instance.IsAnyWindowOpen;

        // При открытом UI: скрываем HUD интеракции и выходим
        if (uiOpen)
        {
            HideHint();
            if (m_PointerImage != null)
                m_PointerImage.gameObject.SetActive(false);
            return;
        }
        else
        {
            if (m_PointerImage != null && !m_PointerImage.gameObject.activeSelf)
                m_PointerImage.gameObject.SetActive(true);
        }

#if ENABLE_INPUT_SYSTEM
        bool interactPressed = Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame;
#else
    bool interactPressed = Input.GetKeyDown(KeyCode.E);
#endif

        bool displayInteractable = false;
        OnInteract[] targets = null;
        GameObject targetGo = null;

        var cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("Camera.main is NULL");
            return;
        }

        var ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, 2.0f))
        {
            var hitGo = hit.collider.gameObject;

            // Ищем OnInteract по родителям от коллайдера
            targets = hit.collider.GetComponentsInParent<OnInteract>(true);
            displayInteractable = targets != null && targets.Length > 0;

            if (displayInteractable)
            {
                // Корень интеракции: ближайший родитель с OnInteract
                var interactRoot = hit.collider.GetComponentInParent<OnInteract>(true);
                targetGo = interactRoot != null ? interactRoot.gameObject : hitGo;

                // DEBUG: раскомментируй на минуту, чтобы увидеть правду
                // Debug.Log($"HIT='{hitGo.name}' tag='{hitGo.tag}' | target='{targetGo.name}' targetTag='{targetGo.tag}' | targets={targets.Length}");

                // Подсказка
                ShowHintFor(targetGo);

                // Указатель
                if (m_PointerImage != null)
                {
                    Sprite chosenPointer = DefaultInteractPointer;

                    if (targetGo.CompareTag("Item")) chosenPointer = ItemPointer;
                    else if (targetGo.CompareTag("Lock")) chosenPointer = LockPointer;
                    else if (targetGo.CompareTag("Tool")) chosenPointer = ToolPointer;
                    else if (targetGo.CompareTag("Box")) chosenPointer = BoxPointer;

                    m_PointerImage.sprite = chosenPointer;
                    m_PointerImage.color = Color.white;
                    m_PointerImage.transform.localScale = m_OriginalPointerSize * 3.0f;

                    // Debug.Log($"Pointer swap: chosen={(chosenPointer ? chosenPointer.name : "NULL")} lock={(LockPointer ? LockPointer.name : "NULL")} default={(DefaultInteractPointer ? DefaultInteractPointer.name : "NULL")} normal={(NormalPointer ? NormalPointer.name : "NULL")}");
                }

                // Взаимодействие
                if (interactPressed)
                {
                    foreach (var t in targets)
                    {
                        if (t != null && t.isActiveAndEnabled)
                            t.Interact();
                    }
                }
            }
            else
            {
                HideHint();
            }
        }
        else
        {
            HideHint();
        }

        // Сброс прицела, если нет интерактива
        if (!displayInteractable && m_PointerImage != null)
        {
            m_PointerImage.sprite = NormalPointer;
            m_PointerImage.color = Color.white;
            m_PointerImage.transform.localScale = m_OriginalPointerSize;
        }
    }

    void ShowHintFor(GameObject hitObject)
    {
        if (titleText == null || actionText == null) return;

        // Название
        var label = hitObject.GetComponentInParent<InteractableLabel>();
        titleText.text = label != null ? label.DisplayName : "";

        // Действие
        var actionProvider = hitObject.GetComponentInParent<IInteractHint>();
        var action = actionProvider != null
            ? actionProvider.GetAction()
            : InteractAction.None;

        actionText.text = ActionToString(action);

        hintRoot.SetActive(true);
    }

    string ActionToString(InteractAction action)
    {
        return action switch
        {
            InteractAction.Take => "[E] Взять",
            InteractAction.Open => "[E] Открыть",
            InteractAction.NeedKey => "Нужен ключ",
            _ => ""
        };
    }

    void HideHint()
    {
        if (hintRoot != null && hintRoot.activeSelf)
            hintRoot.SetActive(false);
    }
}
