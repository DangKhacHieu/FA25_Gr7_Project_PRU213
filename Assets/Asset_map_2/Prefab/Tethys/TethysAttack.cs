using UnityEngine;

public class TethysAttack : MonoBehaviour
{
    [Header("Cài đặt tấn công")]
    public float attackRange = 3f;
    public float fireRate = 1.2f;
    public GameObject bulletPrefab;   // Prefab đạn TethysBolt
    public Transform firePoint;

    [Header("Âm thanh")]
    public AudioClip shootSound;
    private AudioSource audioSource;

    [Header("Sát thương mỗi lần bắn")]
    public int damagePerShot = 60;

    private float fireCountdown = 0f;
    private Transform target;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (target == null)
        {
            FindTarget();
            return;
        }

        // Nếu mục tiêu chết hoặc ra khỏi tầm thì bỏ target
        if (target.GetComponent<EnemyHealth>() == null)
        {
            target = null;
            return;
        }

        float distance = Vector2.Distance(transform.position, target.position);
        if (distance > attackRange)
        {
            target = null;
            return;
        }

        // Quay mặt về phía kẻ địch
        Vector3 scale = transform.localScale;
        scale.x = target.position.x < transform.position.x ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        transform.localScale = scale;

        if (fireCountdown <= 0f)
        {
            Shoot();
            fireCountdown = 1f / fireRate;
        }

        fireCountdown -= Time.deltaTime;
    }

    void FindTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float dist = Vector2.Distance(transform.position, enemy.transform.position);
            if (dist < shortestDistance)
            {
                shortestDistance = dist;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null && shortestDistance <= attackRange)
            target = nearestEnemy.transform;
        else
            target = null;
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        GameObject bulletGO = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        TethysBolt bullet = bulletGO.GetComponent<TethysBolt>();
        if (bullet != null)
        {
            bullet.damage = damagePerShot;
            bullet.SetTarget(target);
        }

        // 🔊 Âm thanh bắn
        if (audioSource != null && shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }
    }
}
