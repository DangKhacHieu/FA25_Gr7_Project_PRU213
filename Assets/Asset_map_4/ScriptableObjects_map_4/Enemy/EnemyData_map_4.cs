using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDataMap4", menuName = "Scriptable Objects/EnemyDataMap4")]
public class EnemyData_map_4 : ScriptableObject
{
    public float lives;
    public int damage;
    public float speed;
    public float resourceReward;

    [Header("Facing Settings")]
    [Tooltip("Nếu sprite gốc nhìn sang phải thì bật = true, nhìn trái thì tắt")]
    public bool artFacesRight;
}
