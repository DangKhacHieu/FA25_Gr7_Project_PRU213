using UnityEngine;

[CreateAssetMenu(fileName = "HeroData_Map5", menuName = "Scriptable Objects/HeroData_Map5")]
public class HeroData_Map5 : ScriptableObject
{
    public float range;
    public float shootInterval;
    public float projectileSpeed;
    public float projectileDuration;
    public float damage;

    public int cost;
    public Sprite sprite;

    public GameObject prefab;

}
