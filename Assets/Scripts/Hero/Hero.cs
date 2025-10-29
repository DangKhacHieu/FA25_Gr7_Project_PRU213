using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Unity.Jobs;

public class Hero : MonoBehaviour
{
    [SerializeField] private HeroData data;
    private CircleCollider2D _circleCollider;

    private List<Enemy> _enemiesInRange;
    private ObjectPooler _projectilePool;

    private float _shootTimer;

    private void Start()
    {
         _circleCollider = GetComponent<CircleCollider2D>();
      _circleCollider.radius = data.range;
        _enemiesInRange = new List<Enemy>();
        _projectilePool = GetComponent<ObjectPooler>();
        _shootTimer = data.shootInterval;
    }

    private void Update()
    {
        _shootTimer -= Time.deltaTime;
        if(_shootTimer <= 0)
        {
            _shootTimer = data.shootInterval;
            shoot();
        }
    }
    private void OnDrawGizmos()
    {
       //Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, data.range);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy")) 
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            _enemiesInRange.Add(enemy);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (_enemiesInRange.Contains(enemy))
            {
                _enemiesInRange.Remove(enemy);
            }
        }
    }


    private void shoot() {
        if (_enemiesInRange.Count > 0) 
        { GameObject projectile = _projectilePool.GetPoolObject();
          projectile.transform.position = transform.position; 
          projectile.SetActive(true); 
          Vector2 _shootDirection = (_enemiesInRange[0].transform.position - transform.position).normalized; 
            projectile.GetComponent<Projecile>().shoot(data, _shootDirection); 
        } 
    }
}

    /*private void ShootLogic()
    {
        List<Enemy> enemies = EnemyManager.Instance.GetActiveEnemies();
        if (enemies == null || enemies.Count == 0) return;

        // Lọc quái trong tầm
        List<Enemy> inRange = new List<Enemy>();
        foreach (var e in enemies)
        {
            if (e == null) continue;
            float dist = Vector3.Distance(transform.position, e.transform.position);
            if (dist <= data.range)
                inRange.Add(e);
        }
        if (inRange.Count == 0) return;

        // Chọn mục tiêu
        Enemy target = SelectTarget(inRange);

        // Kiểm tra buff map băng
        float finalDamage = data.damage;
        float finalShootInterval = data.shootInterval;

        if (data.isMapIceHero && IsIceEnemy(target))
        {
            finalDamage *= data.iceDamageMultiplier;
            finalShootInterval /= data.iceAttackSpeedMultiplier;
        }

        // Bắn đạn
        GameObject projectile = _projectilePool.GetPoolObject();

        if (projectile == null) return;

        projectile.transform.position = transform.position;
        projectile.SetActive(true);

        Vector2 dir = (target.transform.position - transform.position).normalized;
        projectile.GetComponent<Projecile>().shoot(data, dir, finalDamage); // ✅ chỉ 3 tham số

    }*/

   /* private Enemy SelectTarget(List<Enemy> list)
    {
        switch (data.targetPriority)
        {
            case TargetPriority.Closest:
                list.Sort((a, b) =>
                    Vector3.Distance(transform.position, a.transform.position)
                    .CompareTo(Vector3.Distance(transform.position, b.transform.position)));
                break;
            case TargetPriority.Strongest:
                list.Sort((a, b) => b.currentHealth.CompareTo(a.currentHealth));
                break;
            case TargetPriority.First:
            default:
                list.Sort((a, b) => b.distanceTravelled.CompareTo(a.distanceTravelled));
                break;
        }
        return list[0];
    }*/

    /*private bool IsIceEnemy(Enemy enemy)
    {
        return enemy.Data.type == EnemyType.yeti ||
               enemy.Data.type == EnemyType.YetiTanker ||
               enemy.Data.type == EnemyType.PhuThuyBang ||
               enemy.Data.type == EnemyType.SnowMan ||
               enemy.Data.type == EnemyType.BossYeti;
    }*/

