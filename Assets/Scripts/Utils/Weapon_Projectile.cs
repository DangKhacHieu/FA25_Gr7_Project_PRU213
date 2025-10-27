using UnityEngine;

public class Weapon_Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float speed = 10f;
    public float damage = 10f;
    public float lifeTime = 3f;

    [Tooltip("Bù góc xoay của sprite (nếu mũi lao gốc không nhìn theo trục X)")]
    public float rotationOffset = 0f; // 🔥 thêm biến này

    private float timer;
    private Transform target;
    private bool hasHit = false;

    public void Launch(Transform enemyTarget)
    {
        target = enemyTarget;
        timer = lifeTime;
        hasHit = false;
        transform.parent = null;
    }
    private void Awake()
    {
        // Nếu sprite gốc hướng phải thì offset = 0, nếu trái thì 180
        if (GetComponent<SpriteRenderer>() != null)
        {
            rotationOffset = 0f; // hoặc lấy từ dữ liệu prefab nếu muốn tinh chỉnh tự động
        }
    }
    void Update()
    {
        if (hasHit) return;

        if (target == null || !target.gameObject.activeInHierarchy)
        {
            DisableSpear();
            return;
        }

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            DisableSpear();
            return;
        }

        Vector3 dir = (target.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;

        // 🔥 Xoay spear theo hướng bay + bù góc offset
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle + rotationOffset);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (hasHit) return;

        if (col.CompareTag("Enemy"))
        {
            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy != null)
                enemy.TakeDamage(damage);

            transform.parent = col.transform;
            hasHit = true;
            Invoke(nameof(DisableSpear), 0.25f);
        }
    }

    private void DisableSpear()
    {
        CancelInvoke(nameof(DisableSpear));
        if (transform.parent != null)
            transform.parent = null;

        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(DisableSpear));
        target = null;
        hasHit = false;
    }
}
