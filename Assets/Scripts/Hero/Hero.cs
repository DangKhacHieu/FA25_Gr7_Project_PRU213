using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Unity.Jobs;
using System.Linq;

public class Hero : MonoBehaviour
{
    /*[SerializeField] private HeroData data;
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

 


    private void Shoot() {
       

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
    }   */



    [SerializeField] private HeroData data;
    private ObjectPooler _projectilePool;
    private float _shootTimer; // Đếm ngược thời gian bắn

    private void Start()
    {
        _projectilePool = GetComponent<ObjectPooler>();
        _shootTimer = 0; // Bắn ngay khi bắt đầu
    }

    private void Update()
    {
        _shootTimer -= Time.deltaTime;
        if (_shootTimer <= 0)
        {
            // Hàm Shoot() sẽ tự xử lý việc reset _shootTimer
            Shoot();
        }
    }

    private void OnDrawGizmos()
    {
        if (data == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, data.range);
    }

    /// <summary>
    /// Hàm bắn chính, kết hợp logic tìm địch, buff, và reset timer
    /// </summary>
    private void Shoot()
    {
        // 1. Lấy tất cả quái
        List<Enemy> allEnemies = EnemyManager.Instance.GetActiveEnemies();

        // Mặc định reset timer. Nếu không có quái, chờ 1 nhịp.
        // (Bạn có thể đổi thành data.shootInterval nếu muốn)
        float nextShootInterval = 0.25f;

        if (allEnemies == null || allEnemies.Count == 0)
        {
            _shootTimer = nextShootInterval;
            return;
        }

        // 2. Lọc quái trong tầm bắn
        List<Enemy> inRangeEnemies = new List<Enemy>();
        foreach (var e in allEnemies)
        {
            if (e == null || !e.gameObject.activeInHierarchy) continue;
            float dist = Vector3.Distance(transform.position, e.transform.position);
            if (dist <= data.range)
                inRangeEnemies.Add(e);
        }

        if (inRangeEnemies.Count == 0)
        {
            _shootTimer = nextShootInterval;
            return;
        }

        // 3. Chọn mục tiêu dựa trên ưu tiên (Smart, First, v.v.)
        Enemy target = SelectTarget(inRangeEnemies);
        if (target == null)
        {
            _shootTimer = nextShootInterval;
            return;
        }

        // 4. Tính toán Sát thương và Tốc độ đánh (Lấy từ code comment của bạn)
        float finalDamage = data.damage;
        float finalShootInterval = data.shootInterval;

        // Nếu là trụ băng VÀ bắn quái băng -> được buff
        if (data.isMapIceHero && IsIceEnemy(target))
        {
            finalDamage *= data.iceDamageMultiplier;
            finalShootInterval /= data.iceAttackSpeedMultiplier; // Bắn nhanh hơn
        }

        // 5. Reset timer cho phát bắn TIẾP THEO
        _shootTimer = finalShootInterval;

        // 6. Bắn đạn
        GameObject projectile = _projectilePool.GetPoolObject();
        if (projectile == null) return;

        projectile.transform.position = transform.position;
        projectile.SetActive(true);

        Vector2 dir = (target.transform.position - transform.position).normalized;

        // Giả định hàm shoot của bạn có 3 tham số
        projectile.GetComponent<Projecile>().shoot(data, dir, finalDamage);
    }

    /// <summary>
    /// Chọn mục tiêu từ danh sách quái trong tầm bắn
    /// </summary>
    private Enemy SelectTarget(List<Enemy> list)
    {
        // --- LOGIC "SMART" (TÙY TÌNH HUỐNG) ---
        if (data.targetPriority == TargetPriority.Smart)
        {
            Enemy weakestEnemy = null;
            float minHealth = float.MaxValue;

            // Tình huống 1: Tìm quái để "kết liễu" (còn dưới 25% máu)
            foreach (Enemy e in list)
            {
                float healthPercent = e.currentHealth / e.Data.lives;
                if (healthPercent > 0 && healthPercent <= 0.25f)
                {
                    if (e.currentHealth < minHealth)
                    {
                        minHealth = e.currentHealth;
                        weakestEnemy = e;
                    }
                }
            }

            // Nếu tìm thấy một con để kết liễu -> Bắn nó ngay
            if (weakestEnemy != null)
            {
                return weakestEnemy;
            }

            // Tình huống 2: Nếu không có con nào sắp chết, quay về logic "First"
            return GetTargetByPriority(list, TargetPriority.First);
        }

        // --- LOGIC CƠ BẢN (First, Closest, v.v.) ---
        return GetTargetByPriority(list, data.targetPriority);
    }

    /// <summary>
    /// Sắp xếp danh sách và trả về mục tiêu đầu tiên
    /// </summary>
    private Enemy GetTargetByPriority(List<Enemy> list, TargetPriority priority)
    {
        // --- SỬA LỖI LOGIC ---
        // Code của bạn dùng: switch (data.targetPriority)
        // Đã sửa thành: switch (priority)
        // (Để logic "Smart" có thể gọi "First" một cách chính xác)
        switch (priority)
        {
            case TargetPriority.Closest:
                list.Sort((a, b) =>
                    Vector3.Distance(transform.position, a.transform.position)
                    .CompareTo(Vector3.Distance(transform.position, b.transform.position)));
                break;

            case TargetPriority.Strongest:
                list.Sort((a, b) => b.currentHealth.CompareTo(a.currentHealth));
                break;

            case TargetPriority.Weakest:
                list.Sort((a, b) => a.currentHealth.CompareTo(b.currentHealth));
                break;

            case TargetPriority.First:
            default:
                list.Sort((a, b) => b.distanceTravelled.CompareTo(a.distanceTravelled));
                break;
        }

        if (list.Count > 0)
        {
            return list[0]; // Trả về phần tử đầu tiên sau khi đã sắp xếp
        }
        return null;
    }

    /// <summary>
    /// Kiểm tra xem quái có phải loại "Băng" không
    /// </summary>
    private bool IsIceEnemy(Enemy enemy)
    {
        if (enemy == null || enemy.Data == null) return false;

        return enemy.Data.type == EnemyType.yeti ||
               enemy.Data.type == EnemyType.YetiTanker ||
               enemy.Data.type == EnemyType.PhuThuyBang ||
               enemy.Data.type == EnemyType.SnowMan ||
               enemy.Data.type == EnemyType.BossYeti;
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

 
