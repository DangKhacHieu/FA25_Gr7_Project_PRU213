using UnityEngine;

public class SelimAttack : MonoBehaviour
{
    [Header("Cài đặt tấn công")]
    public float attackRange = 3f;
    public float fireRate = 1f;
    public GameObject bulletPrefab;
    public Transform firePoint;

    [Header("Âm thanh")]
    public AudioClip shootSound;   // 👈 thêm field âm thanh bắn
    private AudioSource audioSource;

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

    public int damagePerShot = 75;

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        GameObject bulletGO = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        SandBolt bullet = bulletGO.GetComponent<SandBolt>();
        if (bullet != null)
        {
            bullet.damage = damagePerShot;
            bullet.SetTarget(target);
        }

        // 🔊 Phát âm thanh khi bắn
        if (audioSource != null && shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }
    }
}
