using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Gameplay/Recipe")]
public class Recipe : ScriptableObject
{
    public List<ItemData> inputs = new();
    public List<ItemData> outputs = new();
}
