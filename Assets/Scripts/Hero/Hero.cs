using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Unity.Jobs;
using System.Linq;

public class Hero : MonoBehaviour
{
    [SerializeField] private HeroData data;
  //  private CircleCollider2D _circleCollider;
  //  private List<Enemy> _enemiesInRange;
    private ObjectPooler _projectilePool;

    private float _shootTimer;

    private void Start()
    {
    //     _circleCollider = GetComponent<CircleCollider2D>();
    //  _circleCollider.radius = data.range;
    //    _enemiesInRange = new List<Enemy>();
        _projectilePool = GetComponent<ObjectPooler>();
        _shootTimer = data.shootInterval;
    }

    private void Update()
    {
        _shootTimer -= Time.deltaTime;
        if(_shootTimer <= 0)
        {
            _shootTimer = data.shootInterval;
            Shoot();
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, data.range);
    }

   /* private void OnTriggerEnter2D(Collider2D collision)
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
    }*/


    private void Shoot() {
        /* if (_enemiesInRange.Count > 0) 
         { GameObject projectile = _projectilePool.GetPoolObject();
           projectile.transform.position = transform.position; 
           projectile.SetActive(true); 
           Vector2 _shootDirection = (_enemiesInRange[0].transform.position - transform.position).normalized; 
             projectile.GetComponent<Projecile>().shoot(data, _shootDirection); 
         } */

        // 1. Lấy tất cả quái đang hoạt động từ EnemyManager
        // (Chúng ta sẽ cần tạo script EnemyManager ở bước 3)
        List<Enemy> allEnemies = EnemyManager.Instance.GetActiveEnemies();
        if (allEnemies == null || allEnemies.Count == 0) return;

        // 2. Lọc những quái trong tầm bắn
        List<Enemy> inRangeEnemies = new List<Enemy>();
        foreach (var e in allEnemies)
        {
            if (e == null || !e.gameObject.activeInHierarchy) continue; // Bỏ qua nếu quái đã chết hoặc bị tắt

            float dist = Vector3.Distance(transform.position, e.transform.position);
            if (dist <= data.range)
                inRangeEnemies.Add(e);
        }

        if (inRangeEnemies.Count == 0) return; // Không có quái nào trong tầm

        // 3. Chọn mục tiêu dựa trên ưu tiên
        Enemy target = SelectTarget(inRangeEnemies);
        if (target == null) return;

        // 4. (Tùy chọn) Logic buff sát thương (tôi lấy từ code comment của bạn)
        float finalDamage = data.damage;
        // if (data.isMapIceHero && IsIceEnemy(target))
        // {
        //     finalDamage *= data.iceDamageMultiplier;
        // }

        // 5. Bắn đạn
        GameObject projectile = _projectilePool.GetPoolObject();
        if (projectile == null) return;

        projectile.transform.position = transform.position;
        projectile.SetActive(true);

        Vector2 dir = (target.transform.position - transform.position).normalized;

        // Cập nhật hàm shoot của đạn nếu cần (tôi dùng lại code của bạn)
        // Giả sử hàm shoot của Projecile là: shoot(HeroData heroData, Vector2 direction)
        // Nếu nó cần finalDamage, bạn phải sửa lại hàm đó
        projectile.GetComponent<Projecile>().shoot(data, dir, finalDamage);

        // Nếu Projecile.shoot() nhận 3 tham số như code comment của bạn:
        // projectile.GetComponent<Projecile>().shoot(data, dir, finalDamage);
    }

    // HÀM SELECTTARGET NÂNG CẤP
    private Enemy SelectTarget(List<Enemy> list)
    {
        // --- LOGIC "SMART" (TÙY TÌNH HUỐNG) ---
        if (data.targetPriority == TargetPriority.Smart)
        {
            // Tình huống 1: Tìm quái để "kết liễu"
            // (Ví dụ: quái còn dưới 25% máu)
            Enemy weakestEnemy = null;
            float minHealth = float.MaxValue;

            foreach (Enemy e in list)
            {
                // Cần Enemy.Data để biết máu tối đa (data.lives)
                float healthPercent = e.currentHealth / e.Data.lives;

                if (healthPercent > 0 && healthPercent <= 0.25f) // Còn 25% máu trở xuống
                {
                    if (e.currentHealth < minHealth)
                    {
                        minHealth = e.currentHealth;
                        weakestEnemy = e;
                    }
                }
            }

            // Nếu tìm thấy một con để kết liễu
            if (weakestEnemy != null)
            {
                return weakestEnemy; // ƯU TIÊN KẾT LIỄU
            }

            // Tình huống 2: Nếu không có con nào sắp chết, quay về logic mặc định (ví dụ: First)
            // (Bạn có thể đổi .First thành .Closest nếu muốn)
            return GetTargetByPriority(list, TargetPriority.First);
        }

        // --- LOGIC CŨ (CHO CÁC CHẾ ĐỘ CƠ BẢN) ---
        return GetTargetByPriority(list, data.targetPriority);
    }

    // Kích hoạt lại hàm chọn mục tiêu (từ code comment của bạn)
    private Enemy GetTargetByPriority(List<Enemy> list, TargetPriority priority)
    {
        switch (data.targetPriority)
        {
            case TargetPriority.Closest:
                // Sắp xếp theo khoảng cách tăng dần
                list.Sort((a, b) =>
                    Vector3.Distance(transform.position, a.transform.position)
                    .CompareTo(Vector3.Distance(transform.position, b.transform.position)));
                break;
            case TargetPriority.Strongest:
                // Sắp xếp theo máu giảm dần
                list.Sort((a, b) => b.currentHealth.CompareTo(a.currentHealth));
                break;
            // --- THÊM MỚI ---
            case TargetPriority.Weakest:
                // Sắp xếp theo máu hiện tại (currentHealth) TĂNG DẦN
                list.Sort((a, b) => a.currentHealth.CompareTo(b.currentHealth));
                break;
            // --- HẾT THÊM MỚI ---
            case TargetPriority.First:
            default:
                // Sắp xếp theo quãng đường đi được (distanceTravelled) giảm dần
                // Cần kích hoạt distanceTravelled trong Enemy.cs
                list.Sort((a, b) => b.distanceTravelled.CompareTo(a.distanceTravelled));
                break;
        }
        // Cần kiểm tra list[0] có bị null không
        if (list.Count > 0)
        {
            return list[0];
        }
        return null; // Không tìm thấy mục tiêu
    }   
}

// Bạn cũng cần định nghĩa enum TargetPriority ở đâu đó,
// ví dụ: bên ngoài class Hero hoặc trong file riêng
public enum TargetPriority
{
    First,
    Closest,
    Strongest,
    Weakest,
    Smart
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

