using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    public float lives;
    public int damage;
    public float speed;
    public EnemyType type;

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
}
