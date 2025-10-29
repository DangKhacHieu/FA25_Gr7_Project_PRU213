using UnityEngine;

public class Projecile : MonoBehaviour
{
    private HeroData _data;
    private Vector3 _shootDirection;
    private float _projectileDuration;
    //private float _finalDamage;
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

    public void shoot(HeroData data, Vector3 shootDirection)
    {
        _data = data;
        _shootDirection = shootDirection.normalized;
        _projectileDuration = _data.projectileDuration;
        
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
