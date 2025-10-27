using System;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveData", menuName = "Scriptable Objects/WaveData")]
public class WaveData : ScriptableObject
{
    [Header("Thông tin tổng quát Wave")]
    [Tooltip("Tên mô tả của wave (chỉ để dễ đọc trong Inspector)")]
    public string waveName = "Wave 1";

    [Tooltip("Các nhóm quái được spawn trong cùng wave này")]
    public SubWaveData[] subWaves;
}

[Serializable]
public class SubWaveData
{
    [Header("Cấu hình nhóm quái trong Wave")]
    [Tooltip("Loại quái vật sẽ spawn")]
    public EnemyType enemyType;

    [Tooltip("Số lượng quái trong nhóm này")]
    public int enemyCount = 5;

    [Tooltip("Khoảng thời gian giữa mỗi quái trong nhóm (giây)")]
    public float spawnInterval = 1.0f;

    [Tooltip("Thời gian delay trước khi nhóm này bắt đầu spawn (tính từ khi wave bắt đầu)")]
    public float startDelay = 0f;

    [Header("Cấu hình đường đi (Path)")]
    [Tooltip("Tên đường đi, ví dụ: Path1, Path2, Path3. Sử dụng để tìm trong PathManager.")]
    public string pathName = "Path1";

    [Header("Cấu hình sức mạnh")]
    [Tooltip("Hệ số tăng máu riêng cho nhóm này (1 = bình thường)")]
    public float healthMultiplier = 1f;

    [Tooltip("Hệ số tăng tốc độ di chuyển (1 = bình thường)")]
    public float speedMultiplier = 1f;

    [Tooltip("Nếu bật, nhóm này là Boss wave (để Spawner hoặc UI có thể trigger sự kiện đặc biệt)")]
    public bool isBossGroup = false;
}
