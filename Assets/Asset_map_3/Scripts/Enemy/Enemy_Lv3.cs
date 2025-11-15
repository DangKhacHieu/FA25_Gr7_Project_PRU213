using System;
using System.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI; // <-- THÊM MỚI (Rất quan trọng)

public class Enemy_Lv3 : MonoBehaviour
{
    [SerializeField] private EnemyData_Lv3 data;
    public EnemyData_Lv3 Data => data;
    public static event Action<EnemyData_Lv3> OnEnemyReachedEnd;

    // --- THÊM MỚI ---
    [Header("UI")]
    [SerializeField] private Slider healthBarSlider;
    [SerializeField] private Slider shieldBarSlider; // Kéo thanh giáp vào đây
    // --- HẾT THÊM MỚI ---

    // --- THÊM MỚI ---
    // Event này sẽ thông báo cho Spawner khi quái bị giết
    public static event Action<EnemyData_Lv3> OnEnemyDied;
    // --- HẾT THÊM MỚI ---

    private Path_Lv3 _currentPath;
    
    private Vector3 _targetPosition;
    private int _currentWaypoint;

    public float currentHealth;
    public float distanceTravelled; // Dùng cho logic “First” (quái đi xa nhất)

    // THAY ĐỔI: Chúng ta cần 1 biến tốc độ "hiện tại"
    // để skill "Enrage" có thể thay đổi mà không ảnh hưởng data gốc
    public float currentSpeed;

    // Biến này để theo dõi trạng thái "Enrage" của Người Tuyết
    private bool _hasEnraged = false;
    // --- HẾT THAY ĐỔI ---

    // --- THÊM MỚI: Biến cho Giáp ---
    public float currentShield;
    private bool _isShieldRecharging = false;
    private Coroutine _shieldRegenCoroutine;
    // --- HẾT THÊM MỚI ---

    // --- THÊM MỚI (Dùng cho Boss sau này) ---
    private Coroutine _summonCoroutine;
    // --- HẾT THÊM MỚI ---

    // public bool isDead => currentHealth <= 0;

    /*  private void Awake()
      {
          _currentPath = GameObject.Find("Path1").GetComponent<Path>();
      }*/

    public void SetPath(Path_Lv3 path)
    {
        _currentPath = path;
    }

    private void OnEnable()
    {
        // --- THÊM MỚI ---
        // Rất quan trọng: Reset máu khi quái được tái sử dụng từ pool
        // Tôi giả định EnemyData của bạn có một trường tên là 'lives' hoặc 'health'
        // Dựa trên code cũ của bạn, tôi dùng 'data.lives'

        if (data != null)
        {
            currentHealth = data.lives; // Hoặc data.health tùy bạn đặt tên
            currentSpeed = data.speed;
            _hasEnraged = false;


            // --- Dừng Coroutine cũ (quan trọng cho pooling) ---
            if (_summonCoroutine != null) StopCoroutine(_summonCoroutine);
            if (_shieldRegenCoroutine != null) StopCoroutine(_shieldRegenCoroutine);
            _isShieldRecharging = false;
            // --- HẾT THÊM MỚI ---

            // --- THAY ĐỔI: TÍNH GIÁP 10% ---
            // SỬA LỖI LOGIC GIÁP (Cách 1)
            if (data.type == EnemyType_Lv3.PhuThuyBang)
            {
                // Dùng giá trị từ data, thay vì 10%
                currentShield = data.shieldAmount;
            }
            else
            {
                currentShield = 0;
            }

            // --- THÊM MỚI: Reset thanh máu ---
            if (healthBarSlider != null)
            {
                healthBarSlider.maxValue = data.lives;
                healthBarSlider.value = currentHealth;
                healthBarSlider.gameObject.SetActive(true); // Hiện thanh máu
            }
            // --- HẾT THÊM MỚI ---

            // --- THÊM MỚI: Cập nhật thanh giáp ---
            UpdateShieldBarUI();
            if (shieldBarSlider != null)
            {
                // Chỉ hiện thanh giáp nếu con này CÓ giáp
                bool hasShield = (currentShield > 0);
                shieldBarSlider.gameObject.SetActive(hasShield);
                if (hasShield)
                {
                    shieldBarSlider.maxValue = data.shieldAmount; // Max value là 10% máu
                    shieldBarSlider.value = currentShield; // Set đầy
                }
            }
        }

        // --- HẾT THÊM MỚI ---

        
        // --- Logic của Boss (thêm ở bước sau, để sẵn ở đây) ---
        if (data.type == EnemyType_Lv3.BossYeti)
        {
            if (_summonCoroutine != null) StopCoroutine(_summonCoroutine);
            _summonCoroutine = StartCoroutine(SummonMinionsRoutine());
        }

        // --- SỬA LỖI ÂM THANH ---
        // ĐẶT CODE ÂM THANH XUẤT HIỆN Ở ĐÂY!
        if (data != null && data.spawnSound != null)
        {
            // --- DÒNG TEST MỚI (phát ngay tai người chơi) ---
            AudioSource.PlayClipAtPoint(data.spawnSound, Camera.main.transform.position, 0.5f);
        }

        // --- HẾT ---

        // --- KÍCH HOẠT ĐĂNG KÝ KHI TÁI SỬ DỤNG TỪ POOL ---
        if (EnemyManager_Lv3.Instance != null)
            EnemyManager_Lv3.Instance.RegisterEnemy(this);

        if (_currentPath == null)
        {
            // Thêm kiểm tra an toàn
            Debug.LogError($"Enemy {name} được kích hoạt mà không có Path!");
            return;
        }

        _currentWaypoint = 1;
        _targetPosition = _currentPath.GetPosition(_currentWaypoint);
        transform.position = _targetPosition;

        // --- RESET LẠI QUÃNG ĐƯỜNG ---
        distanceTravelled = 0;
    }

    // --- THÊM HÀM NÀY (RẤT QUAN TRỌNG) ---
    private void OnDisable()
    {
        // Hủy đăng ký khi bị trả về pool
        if (EnemyManager_Lv3.Instance != null)
            EnemyManager_Lv3.Instance.UnregisterEnemy(this);

        // Dừng tất cả coroutine khi bị tắt
        StopAllCoroutines();
        _summonCoroutine = null;
        _shieldRegenCoroutine = null;
        _isShieldRecharging = false;
    }

    // Update is called once per frame
    void Update()
    {
       // ---THAY ĐỔI: Dùng currentSpeed ---
        // float step = data.speed * Time.deltaTime; // <-- Dòng cũ
        float step = currentSpeed * Time.deltaTime; // <-- Dòng MỚI
        // --- HẾT THAY ĐỔI ---

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
        // --- THÊM MỚI: LOGIC GIÁP ---
        // 1. Kiểm tra xem có giáp và giáp còn không
        if (currentShield > 0)
        {
            // Lấy phần sát thương sẽ trừ vào giáp
            float damageToShield = Mathf.Min(damage, currentShield);
            currentShield -= damageToShield;

            // Lấy phần sát thương còn lại (nếu giáp vỡ)
            damage -= damageToShield;

            UpdateShieldBarUI();

            // 2. Nếu giáp VỪA vỡ và CHƯA đang sạc -> bắt đầu sạc
            if (currentShield <= 0 && !_isShieldRecharging)
            {
                _shieldRegenCoroutine = StartCoroutine(RegenerateShield());
            }
        }
        currentHealth -= damage;

        // --- THÊM MỚI: Cập nhật thanh máu ---
        if (healthBarSlider != null)
        {
            healthBarSlider.value = currentHealth;
        }
        // --- HẾT THÊM MỚI ---

        // --- THÊM MỚI: LOGIC "ENRAGE" CỦA NGƯỜI TUYẾT ---
        // Chỉ kích hoạt nếu là Người Tuyết, chưa Enrage, và máu dưới 30%
        if (data.type == EnemyType_Lv3.SnowMan && !_hasEnraged && currentHealth < (data.lives * 0.3f))
        {
            _hasEnraged = true;
            currentSpeed *= 1.25f; // Tăng 25% tốc độ hiện tại
            Debug.Log($"{name} đã nổi điên! Tốc độ mới: {currentSpeed}");
            // TODO: Thêm hiệu ứng hình ảnh (đổi màu, bốc khói...)
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

        if (_summonCoroutine != null)
        {
            StopCoroutine(_summonCoroutine);
            _summonCoroutine = null;
        }
        if (_shieldRegenCoroutine != null)
        {
            StopCoroutine(_shieldRegenCoroutine);
            _shieldRegenCoroutine = null;
        }
        // --- HẾT THÊM MỚI ---

        // 1. Gửi sự kiện cho Spawner biết quái đã chết
        OnEnemyDied?.Invoke(data);

        // --- THÊM MỚI: Thưởng vàng cho người chơi ---
        // Đảm bảo CurrencyManager đã tồn tại trong Scene
        if (CurrencyManager_Lv3.Instance != null)
        {
            // Gọi CurrencyManager và cộng số vàng từ EnemyData
            CurrencyManager_Lv3.Instance.AddGold(data.goldReward);
        }
        else
        {
            Debug.LogWarning($"Không tìm thấy CurrencyManager.Instance để cộng {data.goldReward} vàng!");
        }
        // --- HẾT THÊM MỚI ---

        // TODO: Thưởng vàng, hiển thị hiệu ứng chết...
        // GameManager.Instance.AddGold(data.goldReward);
        if (data != null && data.deathSound != null)
        {
            // Phát tại vị trí Camera để người chơi luôn nghe rõ
            AudioSource.PlayClipAtPoint(data.deathSound, Camera.main.transform.position, 0.8f);
        }

        // --- THÊM MỚI: Ẩn thanh máu ---
        if (healthBarSlider != null)
        {
            healthBarSlider.gameObject.SetActive(false);
        }
        if (shieldBarSlider != null)
        {
            shieldBarSlider.gameObject.SetActive(false);
        }
        // --- HẾT THÊM MỚI ---

        // 2. Trả về object pool
        gameObject.SetActive(false);
    }

    // --- HÀM MỚI: Cập nhật UI thanh giáp ---
    private void UpdateShieldBarUI()
    {
        if (shieldBarSlider != null)
        {
            shieldBarSlider.value = currentShield;
        }
    }

    // --- CHÚNG TA SẼ HOÀN THIỆN HÀM NÀY Ở BƯỚC SAU (CHO BOSS) ---
    private IEnumerator SummonMinionsRoutine()
    {
        // Chờ 15s đầu tiên
        yield return new WaitForSeconds(5f);

        while (currentHealth > 0)
        {
            Debug.Log($"Boss {name} đang triệu hồi lính!");

            // Triệu hồi 3 con "Yeti Thường" (EnemyType.yeti)
            SpawnSingleMinion(EnemyType_Lv3.yeti);
            SpawnSingleMinion(EnemyType_Lv3.yeti);
            SpawnSingleMinion(EnemyType_Lv3.yeti);

            // Chờ 15s cho lần tiếp theo
            yield return new WaitForSeconds(5f);
        }
    }

    private void SpawnSingleMinion(EnemyType_Lv3 typeToSpawn)
    {
        // Dùng dictionary 'static' từ Spawner
        if (Spawner_Lv3.PoolDictionary == null)
        {
            Debug.LogError("Spawner.PoolDictionary chưa được khởi tạo!");
            return;
        }

        if (Spawner_Lv3.PoolDictionary.TryGetValue(typeToSpawn, out ObjectPooler_Lv3 minionPool))
        {
            GameObject minionObj = minionPool.GetPoolObject();
            Enemy_Lv3 minionEnemy = minionObj.GetComponent<Enemy_Lv3>();

            if (minionEnemy != null && _currentPath != null)
            {
                // Gán cùng đường đi của Boss
                minionEnemy.SetPath(_currentPath);
                minionObj.SetActive(true);
            }
            else
            {
                minionObj.SetActive(false); // Trả lại pool nếu lỗi
            }
        }
        else
        {
            Debug.LogWarning($"Không tìm thấy pool cho: {typeToSpawn}");
        }
    }

   /* private void Start()
     {
        // currentHealth = data.lives; // Bạn đã làm việc này trong OnEnable, có thể bỏ ở đây
        if (EnemyManager_Lv3.Instance != null)
            EnemyManager_Lv3.Instance.RegisterEnemy(this);
    }

     private void OnDestroy()
     {
        if (EnemyManager_Lv3.Instance != null)
            EnemyManager_Lv3.Instance.UnregisterEnemy(this);
     }*/

    // --- HÀM MỚI: Coroutine hồi giáp ---
    private IEnumerator RegenerateShield()
    {
        // 1. Đánh dấu là đang sạc
        _isShieldRecharging = true;

        // 2. Chờ (ví dụ: 10 giây)
        yield return new WaitForSeconds(data.shieldRegenTime);

        // 3. Hồi đầy giáp
        currentShield = data.shieldAmount;
        _isShieldRecharging = false;
        _shieldRegenCoroutine = null; // Xóa tham chiếu coroutine

        // 4. Cập nhật UI
        UpdateShieldBarUI();
        Debug.Log($"{name} đã hồi giáp!");
    }


}
