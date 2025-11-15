using System;
using UnityEngine;
[Serializable] // Để Unity Inspector có thể hiển thị
public class EnemyGroup_Map5
{
    public EnemyType enemyType;
    public int enemiesCount;
    public float spawnInterval;
    [Tooltip("Tên GameObject của Path (ví dụ: Path1, Path2)")]
    public string pathName;
}

[CreateAssetMenu(fileName = "WaveData_Map5", menuName = "Scriptable Objects/WaveData_Map5")]
public class WaveData_Map5 : ScriptableObject
{
    public float timeBeforeNextWave = 5f;
    public EnemyGroup_Map5[] enemyGroups;

}
