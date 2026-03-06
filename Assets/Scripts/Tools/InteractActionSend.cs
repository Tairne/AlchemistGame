using UnityEngine;

public class InteractActionSend : MonoBehaviour, IInteractHint
{

    [SerializeField]
    private InteractAction action;

    public InteractAction GetAction()
    {
        return action;
    }
}
