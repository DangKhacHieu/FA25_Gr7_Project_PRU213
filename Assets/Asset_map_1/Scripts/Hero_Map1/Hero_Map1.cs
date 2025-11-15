using System.Collections.Generic;
using UnityEngine;

public class Hero_Map1 : MonoBehaviour
{
    [SerializeField] private HeroData_Map1 data;

    private List<Enemy_Map1> _enemiesInRange;
    private ObjectPooler_Map1 _projectilePool;
    private Enemy_Map1 _currentTarget;

    private float _shootTimer;

    private Animator _animator;
    private AudioSource audioSource;

    private void Start()
    {
        _enemiesInRange = new List<Enemy_Map1>();
        _projectilePool = GetComponent<ObjectPooler_Map1>();

        _shootTimer = data.shootInterval;
        _animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        // Cập nhật danh sách quái dựa trên range
        UpdateEnemiesInRange();

        // Cập nhật target
        if (_currentTarget == null || !_enemiesInRange.Contains(_currentTarget))
        {
            _currentTarget = GetNearestEnemy();
        }

        // Animation
        if (_animator != null)
        {
            _animator.SetBool("isAttacking", _currentTarget != null);
        }

        // Bắn
        if (_currentTarget != null)
        {
            _shootTimer -= Time.deltaTime;
            if (_shootTimer <= 0f)
            {
                _shootTimer = data.shootInterval;
                Shoot(_currentTarget);
            }
        }
    }


    // TÌM QUÁI TRONG TẦM BẮN DỰA TRÊN HeroData.range

    private void UpdateEnemiesInRange()
    {
        _enemiesInRange.Clear();

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, data.range);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy_Map1"))
            {
                Enemy_Map1 enemy = hit.GetComponent<Enemy_Map1>();
                if (enemy != null && enemy.gameObject.activeInHierarchy)
                {
                    _enemiesInRange.Add(enemy);
                }
            }
        }
    }


    // Tìm quái gần nhất

    private Enemy_Map1 GetNearestEnemy()
    {
        Enemy_Map1 nearest = null;
        float minDist = Mathf.Infinity;

        foreach (var enemy in _enemiesInRange)
        {
            if (enemy == null) continue;

            float dist = Vector2.Distance(transform.position, enemy.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = enemy;
            }
        }

        return nearest;
    }


    // BẮN ĐẠN
 
    private void Shoot(Enemy_Map1 target)
    {
        if (target == null) return;

        // Âm thanh bắn
        if (audioSource != null && data.attackSound != null)
        {
            audioSource.PlayOneShot(data.attackSound);
        }

        GameObject projectile = _projectilePool.GetPooledObject();
        if (projectile == null)
        {
            Debug.LogWarning("Không còn projectile trong pool!");
            return;
        }

        projectile.transform.position = transform.position;
        projectile.SetActive(true);

        Vector2 direction = (target.transform.position - transform.position).normalized;

        Projectile_Map1 projScript = projectile.GetComponent<Projectile_Map1>();
        if (projScript == null)
        {
            Debug.LogError("Projectile prefab thiếu script Projectile_Map1!");
            return;
        }

        // TÍNH DAMAGE (bao gồm GIÁP MỎNG)

        float finalDamage = data.damage;

        if (target.Data.kyNang == EnemyData_Map1.KyNang.GiapMong)
        {
            float distance = Vector2.Distance(transform.position, target.transform.position);

            if (distance > 3f)
            {
                finalDamage *= 1.2f; // tăng 20%
            }
        }

        // Bắn với damage cuối cùng
        projScript.Shoot(data, direction, finalDamage);
    }

    
    private void OnDrawGizmosSelected()
    {
        if (data != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, data.range);
        }
    }
}
