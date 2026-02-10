using UnityEngine;

public class InteractableLabel : MonoBehaviour
{
    [SerializeField] private string displayName;
    public string DisplayName => displayName;
}
