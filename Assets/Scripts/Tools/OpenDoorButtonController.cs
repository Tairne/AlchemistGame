using UnityEngine;

public class OpenDoorButtonController : MonoBehaviour, IInteractHint
{
    private bool _isOpen = false;
    [SerializeField] private DoorController _doorController;

    public InteractAction GetAction() => InteractAction.Use;

    public void Push()
    {
        if (!_isOpen)
        {
            _doorController.Open();
            _isOpen = true;
        }
        else
        {
            _doorController.Close();
            _isOpen = false;
        }
    }
}
