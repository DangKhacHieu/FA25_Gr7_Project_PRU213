using UnityEngine;

public class ArcaneBolt : MonoBehaviour
{
    public float speed = 18f;
    public int damage = 80;
    public float maxLifeTime = 4f;

    private Transform target;

    public void SetTarget(Transform t)
    {
        target = t;
        if (target == null) return;

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
        if (other.CompareTag("Bullet")) return;

        if (other.CompareTag("Enemy"))
        {
            var hp = other.GetComponent<EnemyHealth>();
            if (hp != null)
                hp.TakeDamage(damage);

            Destroy(gameObject);
        }
    }
}
