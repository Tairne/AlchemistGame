using UnityEngine;

public class DoorController : MonoBehaviour, IInteractHint
{
    private Animator _anim;
    private bool _isOpen = false;

    public InteractAction GetAction()
    {
        if (CompareTag("Lock"))
        {
            return InteractAction.Closed;
        }
        else
        {
            if (_isOpen)
            {
                return InteractAction.DoorClose;
            }
            else
            {
                return InteractAction.DoorOpen;
            }
        }
    }

    void Awake()
    {
        _anim = GetComponent<Animator>();
    }

    public void ManualControl()
    {
        if (CompareTag("Lock"))
            return;

        if (!_isOpen)
        {
            _anim.SetTrigger("Open");
            _isOpen = true;
        }
        else
        {
            _anim.SetTrigger("Close");
            _isOpen = false;
        }
    }

    public void Open()
    {
        if (!_isOpen)
        {
            _anim.SetTrigger("Open");
            _isOpen = true;
        }           
        
        SetTagRecursively(gameObject, "Tool");
    }

    public void Close()
    {
        if (_isOpen)
        {
            _anim.SetTrigger("Close");
            _isOpen = false;
        }
        
        SetTagRecursively(gameObject, "Lock");
    }

    void SetTagRecursively(GameObject obj, string newTag)
    {
        obj.tag = newTag;

        foreach (Transform child in obj.transform)
        {
            SetTagRecursively(child.gameObject, newTag);
        }
    }
}
