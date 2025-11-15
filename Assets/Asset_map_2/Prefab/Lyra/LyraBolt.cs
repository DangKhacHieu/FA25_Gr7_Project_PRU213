using UnityEngine;

public class LyraBolt : MonoBehaviour
{
    public float speed = 14f;
    public int damage = 60;
    public float maxLifeTime = 4f;
    private Transform target;

    public void SetTarget(Transform t)
    {
        target = t;
        if (target == null)
        {
            Debug.LogWarning($"LyraBolt({name}) → Target NULL khi SetTarget!");
        }
        else
        {
            Vector2 dir = (target.position - transform.position);
            float z = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, z);
            Debug.Log($"LyraBolt({name}) → Set target = {target.name}");
        }
    }

    void Start()
    {
        Destroy(gameObject, maxLifeTime);
        Debug.Log($"LyraBolt({name}) → Spawn thành công, sẽ tự hủy sau {maxLifeTime}s");
    }

    void Update()
    {
        if (target == null)
        {
            Debug.LogWarning($"LyraBolt({name}) → Target bị null, tự hủy!");
            Destroy(gameObject);
            return;
        }

        Vector3 dir = (target.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet")) return;

        if (other.CompareTag("Enemy"))
        {
            Debug.Log($"LyraBolt({name}) → Va chạm với Enemy {other.name}, gây damage {damage}");
            var hp = other.GetComponent<EnemyHealth>();
            if (hp != null)
                hp.TakeDamage(damage);

            Destroy(gameObject);
        }
    }
}
