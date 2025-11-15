using UnityEngine;

[System.Serializable]
public class EnemyGroup
{
    [Header("Loại quái trong nhóm này")]
    public EnemyType_Map1 enemyType;      // Loại quái (chứa prefab, tốc độ, máu,...)

    [Header("Số lượng & tần suất spawn")]
    public int count = 5;            // Số lượng quái trong nhóm
    public float spawnInterval = 1f; // Thời gian giữa mỗi con (giây)

    [Header("Đường đi riêng của nhóm quái này")]
    public Path_Map1 targetPath;          // Mỗi nhóm có thể đi path riêng

    [Header("Tên mô tả (tùy chọn, để debug)")]
    public string pathName;          // Tên hiển thị trong Inspector
}

[CreateAssetMenu(fileName = "WaveData", menuName = "Scriptable Objects/WaveData", order = 1)]
public class WaveData_Map1 : ScriptableObject
{
    [Header("Danh sách các nhóm quái trong Wave này")]
    public EnemyGroup[] enemyGroups;

    [Header("Thời gian delay trước khi Wave bắt đầu (tùy chọn)")]
    public float startDelay = 1f;

    [Header("Tên mô tả Wave (tùy chọn)")]
    public string waveName = "Wave";
}