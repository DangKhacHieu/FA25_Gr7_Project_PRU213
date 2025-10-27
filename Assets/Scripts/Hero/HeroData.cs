// Assets/Scripts/Hero/HeroData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "HeroData", menuName = "Scriptable Objects/HeroData")]
public class HeroData : ScriptableObject
{
    [Header("Presentation")]
    public string displayName;
    public Sprite icon;
    [TextArea] public string description;

    [Header("Placement")]
    public int cost = 100;
    public GameObject prefab;  // Prefab Hero_Spear (hoặc hero khác)

    // (tuỳ ý) có thể thêm stats chung của hero ở đây
}
