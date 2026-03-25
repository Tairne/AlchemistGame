using System.Collections;
using UnityEngine;

public class DoorController : MonoBehaviour, IInteractHint
{
    [SerializeField] private AudioSource doorAudioSource;
    [SerializeField] private AudioSource playerAudioSource;
    [SerializeField] private AudioClip doorSound;
    [SerializeField] private AudioClip lockMechSound;
    [SerializeField] private AudioClip buttonSound;

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
            doorAudioSource.PlayOneShot(doorSound);
        }
        else
        {
            _anim.SetTrigger("Close");
            _isOpen = false;
            doorAudioSource.PlayOneShot(doorSound);
        }
    }

    public void Open()
    {
        if (!_isOpen)
        {
            StartCoroutine(OpenRoutine());
        }

        playerAudioSource.PlayOneShot(buttonSound);
        SetTagRecursively(gameObject, "Tool");
    }

    private IEnumerator OpenRoutine()
    {
        _isOpen = true;

        doorAudioSource.PlayOneShot(lockMechSound);

        yield return new WaitForSeconds(lockMechSound.length);

        _anim.SetTrigger("Open");

        doorAudioSource.PlayOneShot(doorSound);
    }

    public void Close()
    {
        if (_isOpen)
        {
            StartCoroutine(CloseRoutine());
        }

        playerAudioSource.PlayOneShot(buttonSound);
        SetTagRecursively(gameObject, "Lock");
    }

    private IEnumerator CloseRoutine()
    {
        _isOpen = false;

        doorAudioSource.PlayOneShot(lockMechSound);

        yield return new WaitForSeconds(lockMechSound.length);

        _anim.SetTrigger("Close");

        doorAudioSource.PlayOneShot(doorSound);
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
