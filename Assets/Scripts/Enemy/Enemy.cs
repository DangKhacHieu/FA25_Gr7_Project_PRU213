using System;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.UI; // <-- THÊM MỚI (Rất quan trọng)

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyData data;
    public EnemyData Data => data;
    public static event Action<EnemyData> OnEnemyReachedEnd;

    // --- THÊM MỚI ---
    [SerializeField] private Slider healthBarSlider;
    // --- HẾT THÊM MỚI ---

    // --- THÊM MỚI ---
    // Event này sẽ thông báo cho Spawner khi quái bị giết
    public static event Action<EnemyData> OnEnemyDied;
    // --- HẾT THÊM MỚI ---

    private Path _currentPath;
    
    private Vector3 _targetPosition;
    private int _currentWaypoint;

    public float currentHealth;
    public float distanceTravelled; // Dùng cho logic “First” (quái đi xa nhất)
   // public bool isDead => currentHealth <= 0;

    /*  private void Awake()
      {
          _currentPath = GameObject.Find("Path1").GetComponent<Path>();
      }*/

    public void SetPath(Path path)
    {
        _currentPath = path;
    }

    private void OnEnable()
    {
        // --- THÊM MỚI ---
        // Rất quan trọng: Reset máu khi quái được tái sử dụng từ pool
        // Tôi giả định EnemyData của bạn có một trường tên là 'lives' hoặc 'health'
        // Dựa trên code cũ của bạn, tôi dùng 'data.lives'

        /*_currentWaypoint = 0;
        _targetPosition = _currentPath.GetPosition(_currentWaypoint);*/

        /* if (_currentPath == null)
        {
            Debug.LogWarning($"{name} chưa có đường đi được gán!");
            return;
        }*/

        if (data != null)
        {
            currentHealth = data.lives; // Hoặc data.health tùy bạn đặt tên
            // --- THÊM MỚI: Reset thanh máu ---
            if (healthBarSlider != null)
            {
                healthBarSlider.maxValue = data.lives;
                healthBarSlider.value = currentHealth;
                healthBarSlider.gameObject.SetActive(true); // Hiện thanh máu
            }
            // --- HẾT THÊM MỚI ---
        }
        // --- HẾT THÊM MỚI ---

        // --- KÍCH HOẠT ĐĂNG KÝ KHI TÁI SỬ DỤNG TỪ POOL ---
        if (EnemyManager.Instance != null)
            EnemyManager.Instance.RegisterEnemy(this);

        if (_currentPath == null)
        {
            // Thêm kiểm tra an toàn
            Debug.LogError($"Enemy {name} được kích hoạt mà không có Path!");
            return;
        }

        _currentWaypoint = 0;
        _targetPosition = _currentPath.GetPosition(_currentWaypoint);
        transform.position = _targetPosition;

        // --- RESET LẠI QUÃNG ĐƯỜNG ---
        distanceTravelled = 0;
    }

    // Update is called once per frame
    void Update()
    {
       /* if (_currentPath == null) return;
        // move towards target Position
        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, data.speed * Time.deltaTime);*/

        float step = data.speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, step);
        distanceTravelled += step;

        // When target reached, set new target position
        float relativeDistance = (transform.position - _targetPosition).magnitude;
        if(relativeDistance < 0.1f)
        {
            if (_currentWaypoint < _currentPath.Waypoints.Length - 1) 
            {
                _currentWaypoint++;
                _targetPosition = _currentPath.GetPosition(_currentWaypoint);
            }else // reached last waypoint
            {
                OnEnemyReachedEnd?.Invoke(data);
                gameObject.SetActive(false);
            }
           
        }
    }

    // Hàm này sẽ được gọi bởi trụ (Tower)
    public void TakeDamage(float damage)
    {
        // Nếu đã chết rồi thì không trừ nữa
        if (currentHealth <= 0) return;

        currentHealth -= damage;

        // --- THÊM MỚI: Cập nhật thanh máu ---
        if (healthBarSlider != null)
        {
            healthBarSlider.value = currentHealth;
        }
        // --- HẾT THÊM MỚI ---

        // TODO: Hiển thị thanh máu, v.v.

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    private void Die()
    {
        // 1. Gửi sự kiện cho Spawner biết quái đã chết
        OnEnemyDied?.Invoke(data);

        // --- THÊM MỚI: Thưởng vàng cho người chơi ---
        // Đảm bảo CurrencyManager đã tồn tại trong Scene
        if (CurrencyManager.Instance != null)
        {
            // Gọi CurrencyManager và cộng số vàng từ EnemyData
            CurrencyManager.Instance.AddGold(data.goldReward);
        }
        else
        {
            Debug.LogWarning($"Không tìm thấy CurrencyManager.Instance để cộng {data.goldReward} vàng!");
        }
        // --- HẾT THÊM MỚI ---

        // TODO: Thưởng vàng, hiển thị hiệu ứng chết...
        // GameManager.Instance.AddGold(data.goldReward);

        // --- THÊM MỚI: Ẩn thanh máu ---
        if (healthBarSlider != null)
        {
            healthBarSlider.gameObject.SetActive(false);
        }
        // --- HẾT THÊM MỚI ---

        // 2. Trả về object pool
        gameObject.SetActive(false);
    }

     private void Start()
     {
        // currentHealth = data.lives; // Bạn đã làm việc này trong OnEnable, có thể bỏ ở đây
        if (EnemyManager.Instance != null)
            EnemyManager.Instance.RegisterEnemy(this);
    }

     private void OnDestroy()
     {
        if (EnemyManager.Instance != null)
            EnemyManager.Instance.UnregisterEnemy(this);
    }


    /* public void TakeDamage(float damage)
     {
         currentHealth -= damage;
         if (currentHealth <= 0)
         {
             currentHealth = 0;
             EnemyManager.Instance.UnregisterEnemy(this);
             Destroy(gameObject);
         }
     }

     public bool isIceMapEnemy
     {
         get
         {
             return data.type == EnemyType.yeti ||
                    data.type == EnemyType.YetiTanker ||
                    data.type == EnemyType.PhuThuyBang ||
                    data.type == EnemyType.SnowMan ||
                    data.type == EnemyType.BossYeti;
         }
     }*/


}
