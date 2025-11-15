using UnityEngine;

public class SandBolt : MonoBehaviour
{
    public float speed = 12f;
    public int damage = 75;
    public float maxLifeTime = 4f;

    private Transform target;

    public void SetTarget(Transform t)
    {
        target = t;
        Vector2 dir = (target.position - transform.position);
        float z = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, z);
    }

    void Start()
    {
        Destroy(gameObject, maxLifeTime);
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 dir = (target.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // ✅ Bỏ qua va chạm với chính các viên đạn khác
        if (other.CompareTag("Bullet"))
            return;

        // ✅ Chỉ xử lý khi trúng quái
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("SandBolt hit enemy: " + other.name);

            EnemyHealth hp = other.GetComponent<EnemyHealth>();
            if (hp != null)
            {
                hp.TakeDamage(damage);
                Debug.Log("Gây damage " + damage + " cho " + other.name);
            }

            Destroy(gameObject); // hủy đạn sau khi trúng
        }
    }
}
