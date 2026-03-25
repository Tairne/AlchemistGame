using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NoteReaderUI : MonoBehaviour
{
    public static NoteReaderUI Instance { get; private set; }


    [SerializeField] private GameObject root;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text contentText;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip paperSound;

    [Header("Illustration")]
    [SerializeField] private GameObject illustrationContainer;

    [Header("Layout")]
    [SerializeField] private RectTransform contentRect;
    [SerializeField] private ScrollRect scrollRect;

    private GameObject _spawnedIllustration;

    private void Awake()
    {
        Instance = this;
        root.SetActive(false);
    }

    public void Open(NoteContent note)
    {
        if (note == null)
            return;

        titleText.text = note.title;
        contentText.text = note.text;

        ClearIllustration();

        if (note.illustrationPrefab != null)
        {
            illustrationContainer.SetActive(true);
            _spawnedIllustration = Instantiate(note.illustrationPrefab, illustrationContainer.transform);
        }
        else
        {
            illustrationContainer.SetActive(false);
        }

        root.SetActive(true);

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentRect);

        if (scrollRect != null)
            scrollRect.verticalNormalizedPosition = 1f;
        audioSource.PlayOneShot(paperSound);
    }

    public void Close()
    {
        ClearIllustration();
        root.SetActive(false);
    }

    private void ClearIllustration()
    {
        if (_spawnedIllustration != null)
        {
            Destroy(_spawnedIllustration);
            _spawnedIllustration = null;
        }
    }
}
