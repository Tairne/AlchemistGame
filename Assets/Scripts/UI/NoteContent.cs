using UnityEngine;

[System.Serializable]
public class NoteContent
{
    public string title;

    [TextArea(10, 30)]
    public string text;

    public GameObject illustrationPrefab;
}
