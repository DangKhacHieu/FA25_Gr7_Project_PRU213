using UnityEngine;

public class Weapon_Projectile_map_4 : MonoBehaviour
{
    private float speed, damage, lifeTime, timer;
    private Transform target;
    private bool launched;

    // Biến để lưu trữ thông tin đòn đặc biệt
    private bool _isSpecial = false;
    private GameObject _specialEffectPrefab;

    // Hàm Launch nhận thêm 2 tham số mới
    public void Launch(Transform enemy, float newSpeed, float newDamage, float newLifetime, bool isSpecial, GameObject effectPrefab)
    {
        target = enemy;
        launched = true;
        speed = newSpeed;
        damage = newDamage;
        lifeTime = newLifetime;
        timer = lifeTime;

        // Lưu lại thông tin
        _isSpecial = isSpecial;
        _specialEffectPrefab = effectPrefab;
    }

    private void Update()
    {
        if (!launched) return;
        timer -= Time.deltaTime;

        if (target == null || !target.gameObject.activeInHierarchy || timer <= 0f)
        {
            launched = false;
            gameObject.SetActive(false);
            return;
        }

        Vector3 dir = (target.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
    }

    // Reset cờ khi đạn bị tắt (tái sử dụng trong pool)
    private void OnDisable()
    {
        _isSpecial = false;
        _specialEffectPrefab = null;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Enemy")) return;
        var enemy = col.GetComponent<Enemy_map_4>();
        if (enemy == null) return;

        // Kỹ năng Né tránh (Evasion)
        if (enemy.GetComponent<Enemy_Fight_Demon>() != null)
        {
            if (Random.value < 0.25f) // 25% tỷ lệ né
            {
                Debug.Log("FIGHT DEMON NÉ ĐÒN!");
                gameObject.SetActive(false); // Hủy đạn
                return; // Không gây sát thương
            }
        }

        // Kích hoạt hiệu ứng nổ nếu là đòn đặc biệt
        if (_isSpecial && _specialEffectPrefab != null)
        {
            Debug.Log("Tung chiêu đặc biệt! Kích hoạt hiệu ứng!");

            // 🌟 THAY ĐỔI: Lấy vị trí của enemy (col) thay vì vị trí của cây giao (transform)
            Instantiate(_specialEffectPrefab, col.transform.position, Quaternion.identity);
        }

        // Gây sát thương
        enemy.TakeDamage(damage);
        gameObject.SetActive(false);
    }
}