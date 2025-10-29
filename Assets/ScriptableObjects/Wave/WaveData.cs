using UnityEngine;

[CreateAssetMenu(fileName = "WaveData", menuName = "Scriptable Objects/WaveData")]
public class WaveData : ScriptableObject
{
    // Which enemy type???

    public EnemyType enemyTpye;
    public float spawnInterval;
    public int enemiesPerWave;
}
