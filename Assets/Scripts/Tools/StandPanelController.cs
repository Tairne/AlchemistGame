using TMPro;
using UnityEngine;

public class StandPanelController : MonoBehaviour, IInteractHint
{
    public static StandPanelController Instance { get; private set; }

    [Header("Glass Stand Object")]
    [SerializeField] private GameObject glassWalls;
    [Header("UI")]
    [SerializeField] private GameObject standPanel;
    [SerializeField] private TMP_InputField num1;
    [SerializeField] private TMP_InputField num2;
    [SerializeField] private TMP_InputField num3;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip successSound;
    [SerializeField] private AudioClip errorSound;

    public InteractAction GetAction() => InteractAction.Use;
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (standPanel != null) standPanel.SetActive(false);
    }

    public void Open()
    {
        if (standPanel == null)
        {
            Debug.LogError("StandPanelController: UI refs are not assigned", this);
            return;
        }

        var opened = UIWindowsManager.Instance.Open(standPanel);
        audioSource.PlayOneShot(openSound);
        if (!opened)
            return;
    }

    public void Close()
    {
        standPanel.SetActive(false);
        num1.text = "";
        num2.text = "";
        num3.text = "";
        resultText.text = "";
        UIWindowsManager.Instance.CloseAll();
    }

    public void OpenStand()
    {
        bool pass1 = num1.text == "69";
        bool pass2 = num2.text == "77";
        bool pass3 = num3.text == "42";

        if (pass1 && pass2 && pass3)
        {
            glassWalls.SetActive(false);
            resultText.text = "Успешно!";
            resultText.color = Color.green;
            audioSource.PlayOneShot(successSound);
        }
        else
        {
            resultText.text = "Неверно!";
            resultText.color = Color.red;
            audioSource.PlayOneShot(errorSound);
        }
    }
}
