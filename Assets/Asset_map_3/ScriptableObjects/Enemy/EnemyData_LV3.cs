using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData_LV3", menuName = "Scriptable Objects/EnemyData_LV3")]
public class EnemyData_Lv3 : ScriptableObject
{
    public float lives;
    public int damage;
    public float speed;
    public EnemyType_Lv3 type;

    // --- THÊM MỚI ---
    // Thêm trường này để lưu trữ lượng vàng rớt ra
    [Tooltip("Số vàng rớt ra khi quái này bị tiêu diệt")]
    public int goldReward;
    // --- HẾT THÊM MỚI ---

    // --- THÊM MỚI CHO CÁC KỸ NĂNG ---
    [Header("Special Abilities")]
    public bool isSlowImmune; // (Sẽ dùng cho Yeti Băng sau)
    public float shieldAmount; // Dùng cho Pháp Sư (ví dụ: 100)
    public float shieldRegenTime; // Dùng cho Pháp Sư (ví dụ: 10)
    // --- HẾT THÊM MỚI ---

    // --- THÊM VÀO ĐÂY ---
    [Header("Audio")]
    [Tooltip("Tiếng phát ra khi quái bị tiêu diệt")]
    public AudioClip deathSound;
    [Tooltip("Tiếng phát ra khi quái được spawn")]
    public AudioClip spawnSound;
}
