using UnityEngine;

public class TorchFireController : MonoBehaviour, IInteractHint
{
    [SerializeField] private GameObject _light;
    [SerializeField] private GameObject _fire;
    [SerializeField] private Renderer rend;
    [SerializeField] private Material baseMaterial;
    [SerializeField] private Material numMaterial;

    private bool state = false;

    public void ChangeState()
    {
        var mats = rend.materials;

        if (!state) 
        { 
            state = true;
            _light.SetActive(true);
            _fire.SetActive(true);
            mats[1] = numMaterial;
        }
        else
        {
            state = false;
            _light.SetActive(false);
            _fire.SetActive(false);
            mats[1] = baseMaterial;
        }

        rend.materials = mats;
    }

    public InteractAction GetAction()
    {
        if (state)
        {
            return InteractAction.FireOff;
        }
        else
        {
            return InteractAction.FireOn;
        }
    }
}
