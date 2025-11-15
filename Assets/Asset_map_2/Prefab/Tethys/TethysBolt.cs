using UnityEngine;

public class TethysBolt : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 60;
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
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth hp = other.GetComponent<EnemyHealth>();
            if (hp != null)
            {
                hp.TakeDamage(damage);
            }

            // 👇 nếu muốn thêm hiệu ứng va chạm nước (splash):
            // Instantiate(hitEffect, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}
