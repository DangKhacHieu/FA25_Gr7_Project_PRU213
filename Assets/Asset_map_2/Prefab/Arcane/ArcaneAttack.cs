using UnityEngine;

public class ArcaneAttack : MonoBehaviour
{
    [Header("Cài đặt tấn công")]
    public float attackRange = 3.8f;
    public float fireRate = 1.4f;
    public GameObject bulletPrefab;   // gán prefab ArcaneBolt
    public Transform firePoint;       // nơi bắn đạn

    [Header("Sát thương mỗi phát")]
    public int damagePerShot = 80;

    private float fireCountdown = 0f;
    private Transform target;

    void Update()
    {
        fireCountdown -= Time.deltaTime;

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
        if (target.position.x < transform.position.x)
            scale.x = Mathf.Abs(scale.x) * -1;
        else
            scale.x = Mathf.Abs(scale.x);
        transform.localScale = scale;

        if (fireCountdown <= 0f)
        {
            Shoot();
            fireCountdown = 1f / fireRate;
        }
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
        if (bulletPrefab == null || firePoint == null)
        {
            Debug.LogWarning($"{name} thiếu bulletPrefab hoặc firePoint!");
            return;
        }

        GameObject bulletGO = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        var bolt = bulletGO.GetComponent<ArcaneBolt>();

        if (bolt != null)
        {
            bolt.damage = damagePerShot;
            bolt.SetTarget(target);
        }
    }
}
