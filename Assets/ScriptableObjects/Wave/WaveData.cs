using System.Collections.Generic;
using UnityEngine;


// Class mới để định nghĩa một nhóm quái
[System.Serializable]
public class EnemyGroup
{
    public EnemyType enemyType;
    public int count; // Số lượng quái trong nhóm này
    public float spawnInterval; // Thời gian giãn cách spawn giữa các con quái TRONG NHÓM NÀY
    [Tooltip("Thời gian chờ (giây) trước khi bắt đầu spawn nhóm này")]
    public float delayBeforeGroup;
}

[CreateAssetMenu(fileName = "WaveData", menuName = "Scriptable Objects/WaveData")]
public class WaveData : ScriptableObject
{
    // Which enemy type???

    /* public EnemyType enemyTpye;
     public float spawnInterval;
     public int enemiesPerWave;*/

    // --- Code mới ---
    [Header("Wave Settings")]
    public List<EnemyGroup> enemyGroups; // Danh sách các nhóm quái trong wave này
    public int waveGoldReward; // Vàng thưởng (lấy từ cột "Vàng thu")

}
