using UnityEngine;

[CreateAssetMenu(menuName = "Game/Items/Item", fileName = "Item_")]
public class ItemData : ScriptableObject
{
    [Header("Identity")]
    public string id;

    [Header("UI")]
    public string title;

    [TextArea] public string description;
    public Sprite icon;
}
