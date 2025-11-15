using UnityEngine;
using System.Collections.Generic;

public class HealAuraAbility : MonoBehaviour, IAbility
{
    // THAY THẾ: Sử dụng Prefab vòng tròn
    [SerializeField] private GameObject healRingPrefab;

    [SerializeField] private float healInterval = 5f;
    [SerializeField] private float healAmount = 5f;
    [SerializeField] private float range = 3f;

    private Bongma _enemy;
    private float _healTimer;

    private void Awake()
    {
        _enemy = GetComponent<Bongma>();
        if (_enemy != null)
        {
            SetupAbility(_enemy);
            _healTimer = healInterval;
            Debug.Log($"[HealAura] {gameObject.name} (Phuthuybongtoi) đã sẵn sàng. Hồi máu mỗi {healInterval}s.");
        }
    }

    public void SetupAbility(Bongma enemy)
    {
        // Sử dụng Update() để theo dõi thời gian
    }

    private void Update()
    {
        if (_enemy == null || !gameObject.activeInHierarchy) return;

        _healTimer -= Time.deltaTime;
        if (_healTimer <= 0)
        {
            _healTimer = healInterval;
            Debug.Log($"[HealAura] {gameObject.name} kích hoạt Hào quang Hồi phục. Phạm vi: {range}.");
            HealNearbyEnemies();
        }
    }

    private void HealNearbyEnemies()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, range);
        int healedCount = 0;

        foreach (var hit in hitColliders)
        {
            Bongma ally = hit.GetComponent<Bongma>();

            if (ally != null && ally != _enemy && ally.gameObject.activeInHierarchy)
            {
                Debug.Log($"[HealAura] Đã tìm thấy đồng minh {ally.gameObject.name}. Hồi {healAmount} máu.");

                // 🌟 KÍCH HOẠT HIỆU ỨNG VÒNG TRÒN 🌟
                if (healRingPrefab != null)
                {
                    // Sinh ra hiệu ứng tại vị trí của đồng minh
                    Instantiate(healRingPrefab, ally.transform.position, Quaternion.identity);
                }

                ally.TakeDamage(-healAmount);
                healedCount++;
            }
        }

        if (healedCount == 0)
        {
            Debug.Log($"[HealAura] Không có đồng minh nào trong phạm vi {range} để hồi máu.");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}