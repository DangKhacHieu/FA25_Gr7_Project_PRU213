using UnityEngine;

public class Projectile : MonoBehaviour
{
    private HeroData_Map5 _data;
    private Vector3 _shootDirection;
    private float _projectileDuration;


    // Update is called once per frame
    void Update()
    {
        if (_projectileDuration <= 0)
        {
            gameObject.SetActive(false);
        }
        else
        {
            _projectileDuration -= Time.deltaTime;
            transform.position += new Vector3(_shootDirection.x, _shootDirection.y) * _data.projectileSpeed * Time.deltaTime;
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra và xử lý va chạm ngay khi đạn chạm vào quái vật
        if (collision.CompareTag("Bongma"))
        {
            Bongma enemy = collision.GetComponent<Bongma>();

            // Đảm bảo quái vật còn đang hoạt động trước khi gây sát thương
            if (enemy != null && enemy.gameObject.activeInHierarchy)
            {
                enemy.TakeDamage(_data.damage);

                // Vô hiệu hóa đạn NGAY LẬP TỨC khi va chạm
                gameObject.SetActive(false);
            }
        }
    }

    public void Shoot(HeroData_Map5 data, Vector3 shootDirection)
    {
        _data = data;
        _shootDirection = shootDirection;
        _projectileDuration = _data.projectileDuration;
    }

}
