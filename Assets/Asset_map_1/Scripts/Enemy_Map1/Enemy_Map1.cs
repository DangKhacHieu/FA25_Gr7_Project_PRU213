using System;
using UnityEngine;

public class Enemy_Map1 : MonoBehaviour
{
    [SerializeField] private EnemyData_Map1 data;
    public EnemyData_Map1 Data => data;
    public static event Action<EnemyData_Map1> OnEnemyReachedEnd;
    public static event Action<Enemy_Map1> OnEnemyDestroyed;

    private Path_Map1 _currentPath;
    private int _currentWaypointIndex;
    private Transform _targetWaypoint;
    private float _lives;
    private float _maxLives;

    [SerializeField] private Transform healthBar;
    private Vector3 _healthBarOriginalScale;
    private bool _isMoving = false;
    private bool _initialized = false;

    // --- BIẾN MỚI CHO KỸ NĂNG ---
    private float _currentSpeed;          // Tốc độ hiện tại (cho Kỹ năng Nổi Giận)
    private bool _daBiDanhTrung = false;  // Đánh dấu né đòn (cho Kỹ năng Nhanh Nhẹn)
    private float _phanChiaTimer;         // Bộ đếm giờ (cho Kỹ năng Phân Chia)
    // ---------------------------------

    private void Awake()
    {
        if (healthBar != null)
            _healthBarOriginalScale = healthBar.localScale;
    }

    private void OnEnable()
    {
        _isMoving = false;
        _initialized = false;
        if (healthBar != null)
            healthBar.localScale = _healthBarOriginalScale;

        // --- MỚI ---
        // Reset lại trạng thái né đòn mỗi khi quái được tái sử dụng (spawn)
        _daBiDanhTrung = false;
    }

    // Thêm 'startWaypointIndex' để Boss có thể đẻ lính ở vị trí của nó
    public void SetPath(Path_Map1 path, int startWaypointIndex = 0)
    {
        _currentPath = path;

        if (_currentPath == null)
        {
            Debug.LogError($"Enemy '{name}' spawn không có Path hợp lệ!");
            gameObject.SetActive(false);
            return;
        }

        // Sửa: Bắt đầu từ waypoint được chỉ định (mặc định là 0)
        _currentWaypointIndex = startWaypointIndex;
        Transform nextWaypoint = _currentPath.GetWaypoint(_currentWaypointIndex);

        if (nextWaypoint == null)
        {
            Debug.LogError($"Path '{_currentPath.name}' thiếu Waypoint {startWaypointIndex}!");
            gameObject.SetActive(false);
            return;
        }

        // Sửa: Chỉ teleport quái về đầu đường nếu nó là con spawn tự nhiên (index = 0)
        if (startWaypointIndex == 0)
        {
            transform.position = nextWaypoint.position;
        }
        // Nếu là lính do Boss đẻ ra, nó sẽ giữ nguyên vị trí của Boss

        _targetWaypoint = nextWaypoint;
        _isMoving = true;
    }

    private void Update()
    {
        if (!_initialized || !_isMoving || _targetWaypoint == null)
            return;

        // --- ĐÃ SỬA ---
        // Di chuyển bằng tốc độ hiện tại (_currentSpeed) thay vì data.speed
        transform.position = Vector3.MoveTowards(
            transform.position,
            _targetWaypoint.position,
            _currentSpeed * Time.deltaTime // Sửa ở đây
        );

        // --- MỚI: XỬ LÝ KỸ NĂNG BOSS ---
        if (data.kyNang == EnemyData_Map1.KyNang.PhanChia)
        {
            _phanChiaTimer -= Time.deltaTime;
            if (_phanChiaTimer <= 0f)
            {
                SpawnSlimeNho();
                _phanChiaTimer = data.ThoiGianPhanChia; // Reset bộ đếm
            }
        }
        // ------------------------------------

        if (Vector3.Distance(transform.position, _targetWaypoint.position) < 0.05f)
        {
            _currentWaypointIndex++;
            if (_currentWaypointIndex < _currentPath.WaypointCount)
            {
                _targetWaypoint = _currentPath.GetWaypoint(_currentWaypointIndex);
            }
            else
            {
                OnEnemyReachedEnd?.Invoke(data);
                gameObject.SetActive(false);
                _isMoving = false;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        // --- MỚI: KỸ NĂNG NHANH NHẸN (Goblin) ---
        // Chỉ kiểm tra đòn đánh đầu tiên
        if (data.kyNang == EnemyData_Map1.KyNang.NhanhNhen && !_daBiDanhTrung)
        {
            _daBiDanhTrung = true; // Đánh dấu là đã bị nhắm bắn

            if (UnityEngine.Random.value < 0.2f) // 20% cơ hội
            {
                // 🟢 BỎ DẤU // Ở DÒNG DƯỚI ĐỂ DEBUG
                Debug.Log("GOBLIN NÉ ĐÒN THÀNH CÔNG!");
                return; // Né đòn thành công!
            }
            else
            {
                // 🟢 THÊM DÒNG NÀY ĐỂ BIẾT KHI NÓ XỊT
                Debug.Log("Goblin né đòn THẤT BẠI (vẫn nhận damage).");
            }
        }
        // ----------------------------------------

        _lives -= damage;
        _lives = Mathf.Max(_lives, 0);

        UpdateHealthBar();

    
        // Kiểm tra máu sau khi bị trừ
        if (data.kyNang == EnemyData_Map1.KyNang.NoiGian)
        {
            // Nếu máu dưới 50%
            if (_lives / _maxLives < 0.5f)
            {
                _currentSpeed = data.speed * 2f; // Tăng tốc gấp đôi
            }
        }
        // Kiểm tra nếu chết
        if (_lives <= 0)
        {
            

            // 1. Đặt DEBUG LOG để xem nó có vào đây không
            Debug.Log("Enemy  ĐÃ CHẾT! (Máu <= 0)");

            // 2. Kiểm tra xem nó có kỹ năng Phân Chia không
            if (data.kyNang == EnemyData_Map1.KyNang.PhanChia)
            {
                // 3. Đặt DEBUG LOG để xem nó có gọi kỹ năng không
                Debug.Log("Kích hoạt kỹ năng PHÂN CHIA KHI CHẾT!");

                // 4. GỌI HÀM PHÂN CHIA
                SpawnSlimeNho();
            }

            // 5. Báo cho game biết
            OnEnemyDestroyed?.Invoke(this);

            // 6. SAU ĐÓ MỚI TẮT BOSS
            gameObject.SetActive(false);

        }
    }

    private void UpdateHealthBar()
    {
        if (healthBar == null) return;

        float healthPercent = _maxLives > 0 ? _lives / _maxLives : 0f;
        Vector3 scale = _healthBarOriginalScale;
        scale.x = _healthBarOriginalScale.x * healthPercent;
        healthBar.localScale = scale;
    }

    // --- ĐÃ SỬA ---
    public void Initialize(float healthMultiplier)
    {
   
        _maxLives = data.lives * healthMultiplier;
        _lives = _maxLives;


        // 1. Set tốc độ ban đầu
        _currentSpeed = data.speed;

        // 2. Set bộ đếm giờ cho Boss
        if (data.kyNang == EnemyData_Map1.KyNang.PhanChia)
        {
            _phanChiaTimer = data.ThoiGianPhanChia;
        }
        // ----------------------------------

        _initialized = true;
        UpdateHealthBar();
    }

    //Hàm spam của boss
    private void SpawnSlimeNho()
    {
        if (data.PrefabSlimeNho == null)
        {
            Debug.LogWarning("Boss muốn đẻ lính nhưng PrefabSlimeNho bị thiếu!");
            return;
        }

        // 1. Tính hướng "Phía Trước"
        Vector3 forwardDirection = Vector3.zero;
        if (_targetWaypoint != null)
        {
            forwardDirection = (_targetWaypoint.position - transform.position).normalized;
        }
        else
        {
            forwardDirection = transform.up;
        }

        // 2. Hướng "Phía Sau" là ngược lại
        Vector3 backwardDirection = -forwardDirection;

        // 3. Hướng "Bên Phải"
        Vector3 rightDirection = Vector3.Cross(forwardDirection, Vector3.forward).normalized;


        // 4. Định nghĩa khoảng cách (TĂNG 2 SỐ NÀY LÊN)

        float distanceBehind = 1.2f;


        float distanceSideways = 0.6f;




        // 5. Tính 2 vị trí spawn
        Vector3 pos1 = transform.position + (backwardDirection * distanceBehind) - (rightDirection * distanceSideways);
        Vector3 pos2 = transform.position + (backwardDirection * distanceBehind) + (rightDirection * distanceSideways);
        Vector3[] spawnPositions = new Vector3[] { pos1, pos2 };


        for (int i = 0; i < spawnPositions.Length; i++)
        {
            // Tạo Slime Nhỏ tại vị trí đã tính
            GameObject slimeObj = Instantiate(data.PrefabSlimeNho, spawnPositions[i], transform.rotation);

            Enemy_Map1 slimeScript = slimeObj.GetComponent<Enemy_Map1>();
            if (slimeScript != null)
            {
                // Cho Slime Nhỏ đi tiếp con đường của Boss
                slimeScript.SetPath(_currentPath, _currentWaypointIndex);

                // Khởi tạo máu cho Slime Nhỏ (với hệ số 1x)
                slimeScript.Initialize(1f);
                Spawner_Map1.Instance.RegisterSpawnedEnemy();
            }
        }
    }
}