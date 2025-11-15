using UnityEngine;

public class Projecile_Lv3 : MonoBehaviour
{
    private HeroData_Lv3 _data;
    private Vector3 _shootDirection;
    private float _projectileDuration;
    //private float _finalDamage;

    // --- THÊM MỚI ---
    // Biến này sẽ lưu sát thương
    private float _damageToDeal; 
    // --- HẾT THÊM MỚI ---
    void Update()
    {
        if(_projectileDuration <= 0)
        {
            gameObject.SetActive(false);
        }
        else
        {
            _projectileDuration -= Time.deltaTime;
             transform.position += new Vector3(_shootDirection.x, _shootDirection.y) * 
             _data.projectileSpeed * Time.deltaTime;

            //transform.position += _shootDirection * _data.projectileSpeed * Time.deltaTime;
        }
    }

    public void shoot(HeroData_Lv3 data, Vector3 shootDirection, float damage)
    {
        _data = data;
        _shootDirection = shootDirection.normalized;
        _projectileDuration = _data.projectileDuration;
        _damageToDeal = damage; // Lưu sát thương lại

        Debug.Log($"Bắn ra viên đạn với {damage} sát thương");

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy_Lv3 enemy = collision.GetComponent<Enemy_Lv3>();
            if (enemy != null)
            {
                // Dùng sát thương đã được Hero truyền cho
                enemy.TakeDamage(_damageToDeal);
            }

            // Tắt viên đạn sau khi trúng
            gameObject.SetActive(false);
        }
    }

    /* private void OnTriggerEnter2D(Collider2D collision)
     {
         if (collision.CompareTag("Enemy"))
         {
             Enemy enemy = collision.GetComponent<Enemy>();
             if (enemy != null)
             {
                 float damageToDeal = _finalDamage;

                 // Nếu enemy thuộc map băng thì tăng damage
                 if (enemy.isIceMapEnemy)  // ← biến bool này bạn có thể thêm trong script Enemy
                 {
                     damageToDeal *= 1.3f; // tăng 30% damage chẳng hạn
                 }

                 enemy.TakeDamage(damageToDeal);
             }

             gameObject.SetActive(false); // viên đạn biến mất sau khi trúng
         }
     }*/
}
