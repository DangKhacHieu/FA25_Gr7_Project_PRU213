// Assets/Scripts/WaveData_map_4.cs
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveDataMap4", menuName = "Scriptable Objects/WaveDataMap4")]
public class WaveData_map_4 : ScriptableObject
{
    [Header("Thông tin tổng quát Wave")]
    public string waveName = "Wave 1";
    public SubWaveData[] subWaves;
}

[Serializable]
public class SubWaveData
{
    [Header("Cấu hình nhóm quái trong Wave")]
    public EnemyType_map_4 enemyType;
    public int enemyCount = 5;
    public float spawnInterval = 1.0f;
    public float startDelay = 0f;

    [Header("Cấu hình đường đi (Path)")]
    // SỬA LẠI DÒNG NÀY:
    [Tooltip("Tên của GameObject chứa Path_map_4, ví dụ: Path1, Path2")]
    public string pathName = "Path1"; // Đổi về lại string

    [Header("Cấu hình sức mạnh")]
    public float healthMultiplier = 1f;
    public float speedMultiplier = 1f;
    public bool isBossGroup = false;
}