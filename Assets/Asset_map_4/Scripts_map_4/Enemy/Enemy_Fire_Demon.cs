using UnityEngine;

public class Enemy_Fire_Demon : Enemy_map_4
{
    [Header("Fire Demon Skill")]
    [SerializeField] private float rageHealthPercent = 0.3f; // 30% máu
    [SerializeField] private float rageSpeedMultiplier = 2f; // Gấp 2 tốc độ
    [SerializeField] private GameObject rageEffect; // Kéo prefab hiệu ứng (mắt đỏ...) vào đây

    private bool _isRaging = false;

    public override void Initialize(float healthMultiplier, Path_map_4 assignedPath)
    {
        base.Initialize(healthMultiplier, assignedPath);
        // Reset khi hồi sinh
        _isRaging = false;
        if (rageEffect != null) rageEffect.SetActive(false);
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage); // Gọi hàm gốc

        // Nếu chưa "Nộ" và máu dưới 30%
        if (!_isRaging && (_lives / _maxLives) <= rageHealthPercent)
        {
            _isRaging = true;
            Debug.Log("FIRE DEMON NỔI ĐIÊN!");

            // Tăng tốc độ (nhờ biến _currentSpeed ở lớp cha)
            _currentSpeed *= rageSpeedMultiplier;

            if (rageEffect != null) rageEffect.SetActive(true);
        }
    }
}