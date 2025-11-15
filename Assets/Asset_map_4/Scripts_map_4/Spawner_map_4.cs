using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyPoolMapping_map_4
{
    public EnemyType_map_4 enemyType;
    public ObjectPooler_map_4 pool;
}

public class Spawner_map_4 : MonoBehaviour
{
    public static Spawner_map_4 Instance { get; private set; }

    public static event Action<int> OnWaveChanged;
    public static event Action OnMissionComplete;

    [Header("Wave Settings")]
    [SerializeField] private WaveData_map_4[] waves;
    private int _currentWaveIndex = 0;
    private int _waveCounter = 0;
    private WaveData_map_4 CurrentWave => waves[_currentWaveIndex];

    // 🌟 THÊM MỚI: Biến đếm số quái còn sống
    private int _enemiesAlive = 0;
    private bool _allWavesSpawned = false; // Cờ báo đã spawn hết wave cuối

    // 🌟 THÊM MỚI: Biến để lưu số wave cần thắng
    private int _wavesToWin = 0;

    private bool _isBetweenWaves = false;
    private bool _isEndlessMode = false;

    [Header("Enemy Pools")]
    [SerializeField] private List<EnemyPoolMapping_map_4> enemyPools = new List<EnemyPoolMapping_map_4>();
    private Dictionary<EnemyType_map_4, ObjectPooler_map_4> _poolDictionary;

    [Header("Timing")]
    [SerializeField] private float _timeBetweenWaves = 3f;
    private float _waveCooldown;

    [Header("Path Manager Override (optional)")]
    public Transform pathManagerRoot;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _poolDictionary = new Dictionary<EnemyType_map_4, ObjectPooler_map_4>();
        foreach (var mapping in enemyPools)
        {
            if (!_poolDictionary.ContainsKey(mapping.enemyType))
                _poolDictionary.Add(mapping.enemyType, mapping.pool);
        }

        // Xóa code cũ: "if (pathManagerRoot == null)..."
        // vì chúng ta sẽ dùng PathManager_map_4.Instance
    }

    private void OnEnable()
    {
        // 🌟 THÊM LẠI: Lắng nghe sự kiện quái chết/về đích
        Enemy_map_4.OnEnemyReachedEnd += HandleEnemyRemoved;
        Enemy_map_4.OnEnemyDestroyed += HandleEnemyRemoved;
    }

    private void OnDisable()
    {
        // 🌟 THÊM LẠI: Hủy lắng nghe
        Enemy_map_4.OnEnemyReachedEnd -= HandleEnemyRemoved;
        Enemy_map_4.OnEnemyDestroyed -= HandleEnemyRemoved;
    }

    private void Start()
    {
        _enemiesAlive = 0;
        _allWavesSpawned = false;

        // 🌟 THÊM MỚI: Lấy số wave cần thắng từ LevelManager (giả định)
        if (LevelManager.Instance != null && LevelManager.Instance.CurrentLevel != null)
        {
            _wavesToWin = LevelManager.Instance.CurrentLevel.wavesToWin;
        }
        else
        {
            // Fallback: Nếu không có LevelManager, chạy hết mảng
            _wavesToWin = waves.Length;
            Debug.LogWarning("[Spawner] Không tìm thấy LevelManager.Instance.CurrentLevel. Sẽ chạy toàn bộ 'waves.Length'.");
        }

        StartCoroutine(RunWaveRoutine());
    }

    private IEnumerator RunWaveRoutine()
    {
        yield return new WaitForSeconds(2f);

        // 🌟 THAY ĐỔI: Vòng lặp chạy cho đến khi đủ số wave thắng
        while (_waveCounter < _wavesToWin || _isEndlessMode)
        {
            if (_isEndlessMode)
            {
                Debug.Log("_isEndlessMode");
            }
            WaveData_map_4 wave = CurrentWave;
            _waveCounter++;
            OnWaveChanged?.Invoke(_waveCounter);

            Debug.Log($"[Spawner] 🚀 Bắt đầu Wave {_waveCounter}/{_wavesToWin}");

            List<Coroutine> runningSpawners = new List<Coroutine>();
            foreach (SubWaveData sub in wave.subWaves)
            {
                if (_isEndlessMode)
                {
                    sub.healthMultiplier = _waveCounter;
                    sub.speedMultiplier = _waveCounter;
                }
                runningSpawners.Add(StartCoroutine(SpawnSubWave(sub)));
            }

            // Chờ cho tất cả các coroutine 'SpawnSubWave' chạy xong
            foreach (Coroutine spawner in runningSpawners)
            {
                yield return spawner;
            }

            Debug.Log($"[Spawner] ✅ Đã spawn hết quái Wave {_waveCounter}.");

            // 🌟 THAY ĐỔI LOGIC:
            // Kiểm tra xem đây có phải là wave cuối cùng CẦN THẮNG không
            if (_waveCounter >= _wavesToWin && !_isEndlessMode)
            {
                Debug.Log($"[Spawner] 🏁 ĐÃ SPAWN HẾT WAVE CUỐI ({_waveCounter}). Chờ tiêu diệt hết quái...");
                _allWavesSpawned = true;

                // Kiểm tra ngay 1 lần, phòng trường hợp quái chết hết trước cả khi check
                CheckForMissionComplete();
                yield break; // Dừng vòng lặp spawn
            }

            // Nghỉ giữa 2 wave
            _isBetweenWaves = true;
            _waveCooldown = _timeBetweenWaves;
            Debug.Log($"[Spawner] Bắt đầu nghỉ {_waveCooldown} giây...");
            yield return new WaitForSeconds(_waveCooldown);
            _isBetweenWaves = false;

            // Lấy wave tiếp theo (dùng modulo % để lặp lại nếu là endless)
            _currentWaveIndex = (_currentWaveIndex + 1) % waves.Length;
        }
    }

    private IEnumerator SpawnSubWave(SubWaveData sub)
    {
        yield return new WaitForSeconds(sub.startDelay);

        if (!_poolDictionary.TryGetValue(sub.enemyType, out var pool))
        {
            Debug.LogWarning($"⚠️ Không tìm thấy pool cho enemy type: {sub.enemyType}");
            yield break;
        }

        // 🌟 SỬA: Dùng PathManager_map_4.Instance để lấy Path
        // (Dựa trên file PathManager_map_4.cs bạn đã gửi)
        Path_map_4 assignedPath = PathManager_map_4.Instance.GetPathByName(sub.pathName);

        if (assignedPath == null)
        {
            // GetPathByName đã log lỗi, không cần log lại
            yield break;
        }

        for (int i = 0; i < sub.enemyCount; i++)
        {
            GameObject obj = pool.GetPooledObject();
            obj.transform.position = assignedPath.GetPosition(0);

            obj.SetActive(true);
            Enemy_map_4 enemy = obj.GetComponent<Enemy_map_4>();
            enemy.Initialize(sub.healthMultiplier, assignedPath);

            // 🌟 THÊM MỚI: Tăng số quái còn sống
            _enemiesAlive++;

            yield return new WaitForSeconds(sub.spawnInterval);
        }
    }

    // 🌟 THÊM LẠI: Hàm xử lý khi quái bị xóa
    private void HandleEnemyRemoved(Enemy_map_4 enemy)
    {
        if (_enemiesAlive > 0) _enemiesAlive--;
        CheckForMissionComplete();
    }

    private void HandleEnemyRemoved(EnemyData_map_4 data) // Xử lý khi quái về đích
    {
        if (_enemiesAlive > 0) _enemiesAlive--;
        CheckForMissionComplete();
    }

    // 🌟 THÊM MỚI: Hàm kiểm tra chiến thắng
    private void CheckForMissionComplete()
    {
        // Điều kiện thắng:
        // 1. Không phải chế độ Endless
        // 2. Đã spawn hết tất cả các wave (theo 'wavesToWin')
        // 3. Không còn quái nào sống
        if (!_isEndlessMode && _allWavesSpawned && _enemiesAlive <= 0)
        {
            OnMissionComplete?.Invoke();

            // Đặt lại cờ để tránh gọi 2 lần
            _allWavesSpawned = false;
        }
    }

    public void EnableEndlessMode()
    {
        _isEndlessMode = true;
        StartCoroutine(RunWaveRoutine());
    }
}