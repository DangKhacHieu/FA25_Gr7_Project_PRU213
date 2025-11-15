using UnityEngine;

public class LyraAttack : MonoBehaviour
{
    [Header("Cài đặt tấn công")]
    public float attackRange = 3.2f;
    public float fireRate = 1.2f;
    public GameObject bulletPrefab;   // gán prefab LyraBolt
    public Transform firePoint;       // empty child nơi bắn đạn

    [Header("Sát thương mỗi phát")]
    public int damagePerShot = 60;

    private float fireCountdown = 0f;
    private Transform target;

    void Update()
    {
        // Đưa countdown lên đầu để đảm bảo giảm mỗi frame
        fireCountdown -= Time.deltaTime;

        if (target == null)
        {
            FindTarget();
            return;
        }

        // Nếu mục tiêu đã bị destroy (không còn EnemyHealth)
        if (target.GetComponent<EnemyHealth>() == null)
        {
            Debug.Log($"{name} → Mục tiêu bị destroy, reset target");
            target = null;
            return;
        }

        // Ngoài tầm thì bỏ mục tiêu
        float distance = Vector2.Distance(transform.position, target.position);
        if (distance > attackRange)
        {
            Debug.Log($"{name} → Mục tiêu ra khỏi tầm ({distance})");
            target = null;
            return;
        }

        // Lật trái/phải theo vị trí quái
        Vector3 scale = transform.localScale;
        if (target.position.x < transform.position.x)
            scale.x = Mathf.Abs(scale.x) * -1;
        else
            scale.x = Mathf.Abs(scale.x);
        transform.localScale = scale;

        // Debug đường nối
        Debug.DrawLine(transform.position, target.position, Color.red);

        // Bắn theo fireRate
        if (fireCountdown <= 0f)
        {
            Debug.Log($"{name} → Đang bắn vào {target.name}");
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
        {
            target = nearestEnemy.transform;
            Debug.Log($"{name} → Tìm thấy mục tiêu {nearestEnemy.name} ở khoảng cách {shortestDistance}");
        }
        else
        {
            target = null;
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null)
        {
            Debug.LogWarning($"{name} thiếu bulletPrefab hoặc firePoint!");
            return;
        }

        GameObject bulletGO = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        var bolt = bulletGO.GetComponent<LyraBolt>();

        if (bolt != null)
        {
            bolt.damage = damagePerShot;
            bolt.SetTarget(target);
            Debug.Log($"{name} → Tạo đạn LyraBolt và truyền target = {target.name}");
        }
        else
        {
            Debug.LogWarning($"{name} → Không tìm thấy LyraBolt component!");
        }
    }
}
