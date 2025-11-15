using UnityEngine;

public class Projectile_Map1 : MonoBehaviour
{
    private HeroData_Map1 _data;
    private Vector2 _shootDirection;
    private float _projectileDuration;

    // --- MỚI ---
    private float _calculatedDamage; // Lưu sát thương đã được tính toán

    private void Update()
    {
        if (_data == null) return;

        if (_projectileDuration <= 0)
        {
            gameObject.SetActive(false);
            return;
        }

        _projectileDuration -= Time.deltaTime;
        transform.Translate(_shootDirection * _data.projectileSpeed * Time.deltaTime, Space.World);

        // (Optional) Xoay đầu đạn theo hướng bay
        float angle = Mathf.Atan2(_shootDirection.y, _shootDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    // --- HÀM NÀY ĐÃ ĐƯỢC SỬA ---
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy_Map1"))
        {
            Enemy_Map1 enemy = collision.GetComponent<Enemy_Map1>();
            if (enemy != null)
            {
                // Sửa: Gây sát thương bằng _calculatedDamage
                // thay vì _data.damage
                enemy.TakeDamage(_calculatedDamage);
            }
            gameObject.SetActive(false);
        }
    }

    // --- HÀM NÀY ĐÃ ĐƯỢC SỬA ---
    // Thêm tham số "float calculatedDamage"
    public void Shoot(HeroData_Map1 data, Vector2 shootDirection, float calculatedDamage)
    {
        _data = data;
        _shootDirection = shootDirection.normalized;
        _projectileDuration = _data.projectileDuration;

        // --- MỚI ---
        _calculatedDamage = calculatedDamage; // Lưu lại sát thương
    }
}