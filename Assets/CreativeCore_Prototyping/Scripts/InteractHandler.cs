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

    [Header("Drag")]
    public Sprite FistPointer;                 // иконка кулака
    [SerializeField] private LayerMask dragLayers = ~0;

    private DragInteractable _dragTarget;
    private Rigidbody _dragRb;
    private SpringJoint _dragJoint;

    private GameObject _dragAnchorGO;
    private Rigidbody _dragAnchorRb;

    private float _dragTimeLeft;
    private float _dragDistance;
    private bool _isDragging;
    private RigidbodyConstraints _savedConstraints;
    private FixedJoint _dragFixedJoint;
    private Vector3 _anchorTargetPos;
    private bool _savedUseGravity;

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

        CreateDragAnchor();
    }

    void FixedUpdate()
    {
        if (_isDragging && _dragAnchorRb != null)
            _dragAnchorRb.MovePosition(_anchorTargetPos);
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
        bool lmbDown = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
        bool lmbUp = Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame;
#else
bool lmbDown = Input.GetMouseButtonDown(0);
bool lmbUp   = Input.GetMouseButtonUp(0);
#endif

        if (_isDragging)
        {
            UpdateDragging();

            // таймер тикает только когда не пауза
            _dragTimeLeft -= Time.deltaTime;

            if (lmbUp || _dragTimeLeft <= 0f)
                EndDrag();

            // пока тащим: никакие другие интеракты не обрабатываем
            return;
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
            var drag = hit.collider.GetComponentInParent<DragInteractable>();

            if (drag != null && drag.CanDrag)
            {
                if (m_PointerImage != null && FistPointer != null)
                    m_PointerImage.sprite = FistPointer; // опционально даже до нажатия

                if (lmbDown)
                {
                    BeginDrag(drag, hit);
                    return; // не запускаем обычные интеракты в этот кадр
                }
            }

            var hitGo = hit.collider.gameObject;

            // Ищем OnInteract по родителям от коллайдера
            targets = hit.collider.GetComponentsInParent<OnInteract>(true);
            displayInteractable = targets != null && targets.Length > 0;

            if (displayInteractable)
            {
                // Корень интеракции: ближайший родитель с OnInteract
                var interactRoot = hit.collider.GetComponentInParent<OnInteract>(true);
                targetGo = interactRoot != null ? interactRoot.gameObject : hitGo;

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
        if (hintRoot == null || titleText == null || actionText == null) return;

        var label = hitObject.GetComponentInParent<InteractableLabel>();
        titleText.text = label != null ? label.DisplayName : hitObject.name;

        // 1) если тащим прямо сейчас
        if (_isDragging)
        {
            actionText.text = "[ЛКМ] Переместить";
            hintRoot.SetActive(true);
            return;
        }

        // 2) если объект можно тащить
        var drag = hitObject.GetComponentInParent<DragInteractable>();
        if (drag != null && drag.CanDrag)
        {
            actionText.text = "[ЛКМ] Переместить";
            hintRoot.SetActive(true);
            return;
        }

        // 3) обычная логика подсказок
        var actionProvider = hitObject.GetComponentInParent<IInteractHint>();
        var action = actionProvider != null ? actionProvider.GetAction() : InteractAction.None;

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
            InteractAction.Use => "[E] Использовать",
            InteractAction.Closed => "Закрыто",
            InteractAction.DoorClose => "[E] Закрыть",
            InteractAction.DoorOpen => "[E] Открыть",
            _ => "Закрыто"
        };
    }

    void HideHint()
    {
        if (hintRoot != null && hintRoot.activeSelf)
            hintRoot.SetActive(false);
    }

    void CreateDragAnchor()
    {
        if (_dragAnchorGO != null && _dragAnchorRb != null) return;

        if (_dragAnchorGO == null)
        {
            _dragAnchorGO = new GameObject("DragAnchor");
            _dragAnchorGO.hideFlags = HideFlags.HideInHierarchy;
        }

        if (_dragAnchorRb == null)
        {
            _dragAnchorRb = _dragAnchorGO.GetComponent<Rigidbody>();
            if (_dragAnchorRb == null) _dragAnchorRb = _dragAnchorGO.AddComponent<Rigidbody>();

            _dragAnchorRb.isKinematic = true;
            _dragAnchorRb.useGravity = false;
            _dragAnchorRb.interpolation = RigidbodyInterpolation.Interpolate;
            _dragAnchorRb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        }
    }

    void BeginDrag(DragInteractable drag, RaycastHit hit)
    {
        if (drag == null) return;

        CreateDragAnchor();

        // 1) Берём Rigidbody от коллайдера
        _dragRb = hit.rigidbody;

        // 2) Фоллбек: ищем рядом (на всякий)
        if (_dragRb == null)
            _dragRb = drag.GetComponentInParent<Rigidbody>();

        if (_dragRb == null)
        {
            Debug.LogError($"BeginDrag: Rigidbody NOT found. " +
                           $"Hit='{hit.collider.name}' tag='{hit.collider.tag}'. " +
                           $"Add Rigidbody to the object (or parent) that has this collider.", hit.collider);
            return;
        }

        _dragTarget = drag;

        // теперь безопасно
        _savedUseGravity = _dragRb.useGravity;
        _dragRb.useGravity = false;

        _savedConstraints = _dragRb.constraints;
        _dragRb.constraints |= RigidbodyConstraints.FreezeRotation;

        drag.PrepareRigidbodyForDrag(_dragRb);

        var cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("BeginDrag: Camera.main is null. Check MainCamera tag.");
            return;
        }

        _dragTimeLeft = Mathf.Max(0.1f, drag.maxDragSeconds);

        _dragAnchorGO.transform.position = _dragRb.worldCenterOfMass;
        _dragDistance = Vector3.Distance(cam.transform.position, _dragRb.worldCenterOfMass);

        if (_dragFixedJoint != null) Destroy(_dragFixedJoint);
        _dragFixedJoint = _dragRb.gameObject.AddComponent<FixedJoint>();
        _dragFixedJoint.connectedBody = _dragAnchorRb;
        _dragFixedJoint.breakForce = Mathf.Infinity;
        _dragFixedJoint.breakTorque = Mathf.Infinity;

        _isDragging = true;

        if (m_PointerImage != null && FistPointer != null)
        {
            m_PointerImage.sprite = FistPointer;
            m_PointerImage.transform.localScale = m_OriginalPointerSize * 3.0f;
        }

        if (hintRoot != null) hintRoot.SetActive(true);
        if (actionText != null) actionText.text = "[ЛКМ] Переместить";
    }

    void UpdateDragging()
    {
        if (_dragAnchorGO == null) return;

        var cam = Camera.main;
        _anchorTargetPos = cam.transform.position + cam.transform.forward * _dragDistance;

        if (actionText != null) actionText.text = "[ЛКМ] Переместить";
    }

    void EndDrag()
    {
        if (_dragRb != null)
        {
            _dragRb.constraints = _savedConstraints;
            _dragRb.useGravity = _savedUseGravity;
        }
            
        if (_dragFixedJoint != null) Destroy(_dragFixedJoint);
        _dragFixedJoint = null;

        _isDragging = false;

        if (_dragJoint != null)
            Destroy(_dragJoint);

        _dragJoint = null;
        _dragRb = null;
        _dragTarget = null;

        // вернуть обычный курсор
        if (m_PointerImage != null)
        {
            m_PointerImage.sprite = NormalPointer;
            m_PointerImage.color = Color.white;
            m_PointerImage.transform.localScale = m_OriginalPointerSize;
        }
    }
}
