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
}
