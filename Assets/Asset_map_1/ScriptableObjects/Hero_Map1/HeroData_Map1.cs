using UnityEngine;

[CreateAssetMenu(fileName = "HeroData_Map1", menuName = "Scriptable Objects/HeroData_Map1")]
public class HeroData_Map1 : ScriptableObject
{
    public string displayName;
    public float range;
    public float shootInterval;
    public float projectileSpeed;
    public float projectileDuration;
    public float damage;

    public int cost;
    public Sprite sprite;

    // ----- THÊM DÒNG NÀY -----
    public AudioClip attackSound;

    public GameObject prefab;
}
