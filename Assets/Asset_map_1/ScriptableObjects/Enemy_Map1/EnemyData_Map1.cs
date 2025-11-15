using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData_Map1", menuName = "Scriptable Objects/EnemyData_Map1")]
public class EnemyData_Map1 : ScriptableObject
{
    // --- NỘI DUNG GỐC CỦA BẠN (Giữ nguyên) ---
    public float lives;
    public int damage;
    public float speed;
    public float resourceReward;

    // --- NỘI DUNG VIẾT THÊM ĐỂ QUẢN LÝ KỸ NĂNG ---

    // 1. Định nghĩa các loại kỹ năng
    public enum KyNang
    {
        KhongCo,      // Dành cho Slime thường
        NhanhNhen,    // Dành cho Goblin (Né đòn)
        NoiGian,      // Dành cho Slime Phản Nộ (Tăng tốc khi ít máu)
        GiapMong,     // Dành cho Goblin Cung Thủ (Yếu khi bị bắn xa)
        PhanChia      // Dành cho Boss Slime Khổng Lồ (Đẻ lính)
    }

    [Header("Kỹ năng đặc biệt")]
    // 2. Biến để chọn kỹ năng trong Inspector
    public KyNang kyNang;

    [Header("Chỉ dành cho BOSS (Kỹ năng Phân Chia)")]
    // 3. Biến cho kỹ năng 'PhanChia' của Boss
    public GameObject PrefabSlimeNho; // Prefab của Slime nhỏ
    public float ThoiGianPhanChia = 10f; // Thời gian Boss đẻ 1 lần
}