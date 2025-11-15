using System.Collections.Generic;
using UnityEngine;


// Class mới để định nghĩa một nhóm quái
[System.Serializable] 
public class EnemyGroup_LV3
{
    public EnemyType_Lv3 enemyType;
    public int count; // Số lượng quái trong nhóm này
    public float spawnInterval; // Thời gian giãn cách spawn giữa các con quái TRONG NHÓM NÀY
    [Tooltip("Thời gian chờ (giây) trước khi bắt đầu spawn nhóm này")]
    public float delayBeforeGroup;
}

[CreateAssetMenu(fileName = "WaveData_LV3", menuName = "Scriptable Objects/WaveData_LV3")]
public class WaveData_Lv3 : ScriptableObject
{
    // Which enemy type???

    /* public EnemyType enemyTpye;
     public float spawnInterval;
     public int enemiesPerWave;*/

    // --- Code mới ---
    [Header("Wave Settings")]
    public List<EnemyGroup_LV3> enemyGroups; // Danh sách các nhóm quái trong wave này
    public int waveGoldReward; // Vàng thưởng (lấy từ cột "Vàng thu")

}
