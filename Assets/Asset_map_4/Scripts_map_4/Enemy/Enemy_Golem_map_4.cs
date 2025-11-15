using System.Collections;
using UnityEngine;

public class Enemy_Golem_map_4 : Enemy_map_4
{
    [Header("Golem Skill")]
    [SerializeField] private float shieldHealthPercent = 0.5f; // 50% máu
    [SerializeField] private float shieldDuration = 3f; // 3 giây bất tử
    [SerializeField] private GameObject shieldEffect; // Kéo prefab hiệu ứng khiên vào đây

    private bool _shieldUsed = false;
    private bool _isInvulnerable = false;

    public override void Initialize(float healthMultiplier, Path_map_4 assignedPath)
    {
        base.Initialize(healthMultiplier, assignedPath);
        // Reset khiên khi hồi sinh từ pool
        _shieldUsed = false;
        _isInvulnerable = false;
        if (shieldEffect != null) shieldEffect.SetActive(false);
    }

    public override void TakeDamage(float damage)
    {
        // Nếu đang bất tử, không nhận sát thương
        if (_isInvulnerable || _isDead) return;

        // Gọi hàm TakeDamage gốc (để trừ máu, cập nhật UI...)
        base.TakeDamage(damage);

        // Nếu máu dưới 50% và chưa dùng khiên
        if (!_shieldUsed && (_lives / _maxLives) <= shieldHealthPercent)
        {
            _shieldUsed = true;
            StartCoroutine(ActivateMagmaShield());
        }
    }

    private IEnumerator ActivateMagmaShield()
    {
        Debug.Log("GOLEM KÍCH HOẠT KHIÊN!");
        _isInvulnerable = true;
        if (shieldEffect != null) shieldEffect.SetActive(true);

        yield return new WaitForSeconds(shieldDuration);

        _isInvulnerable = false;
        if (shieldEffect != null) shieldEffect.SetActive(false);
        Debug.Log("GOLEM TẮT KHIÊN!");
    }
}