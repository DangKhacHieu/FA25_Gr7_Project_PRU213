using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Spawner_Map5 : MonoBehaviour
{
    public static Spawner_Map5 Instance { get; private set; }

    public static event Action<int> OnWaveChanged;
    public static event Action OnMissionComplete;

    [SerializeField] private WaveData_Map5[] waves;
    private int _currentWaveIndex = 0;
    private int _waveCounter = 0;

    // Quản lý trạng thái sinh quái cho Wave hiện tại
    private List<EnemyGroup_Map5> _currentWaveGroups;
    private int _totalEnemiesInWave;
    private int _enemiesSpawnedInWave;
    private int _enemiesRemovedInWave; // Giữ lại cho mục đích kiểm tra điều kiện thắng

    private float _groupSpawnTimer;
    private int _currentGroupIndex = 0;
    private int _enemiesSpawnedInCurrentGroup = 0;
    private bool _isSpawningGroup = false;


    [SerializeField] private ObjectPooler BongmaPool;
    [SerializeField] private ObjectPooler BongmaacdocPool;
    [SerializeField] private ObjectPooler XuongPool;
    [SerializeField] private ObjectPooler PhuthuybongtoiPool;
    [SerializeField] private ObjectPooler ChuatebongtoiPool;


    private Dictionary<EnemyType, ObjectPooler> _poolDictionary;
    private Dictionary<string, Path> _pathDictionary;

    private float _waveCooldown;
    private bool _isBetweenWaves = true;
    private bool _isEndlessMode = false;

    private void Awake()
    {
        _poolDictionary = new Dictionary<EnemyType, ObjectPooler>()
        {
            { EnemyType.Bongma, BongmaPool},
            { EnemyType.Bongmaacdoc, BongmaacdocPool},
            { EnemyType.Xuong, XuongPool},
            { EnemyType.Phuthuybongtoi, PhuthuybongtoiPool},
            { EnemyType.Chuatebongtoi, ChuatebongtoiPool},
        };

        // TÌM VÀ LƯU TRỮ TẤT CẢ CÁC PATH
        _pathDictionary = new Dictionary<string, Path>();
        Path[] allPaths = FindObjectsOfType<Path>();
        foreach (Path path in allPaths)
        {
            if (!_pathDictionary.ContainsKey(path.gameObject.name))
            {
                _pathDictionary.Add(path.gameObject.name, path);
            }
            else
            {
                Debug.LogError($"Multiple Paths found with the name: {path.gameObject.name}. Please ensure path names are unique.");
            }
        }

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    private void OnEnable()
    {
        Bongma.OnEnemyReachedEnd += HandleEnemyReachedEnd;
        Bongma.OnEnemyDestroyed += HandleEnemyDestroyed;

    }

    private void OnDisable()
    {
        Bongma.OnEnemyReachedEnd -= HandleEnemyReachedEnd;
        Bongma.OnEnemyDestroyed -= HandleEnemyDestroyed;

    }

    private void Start()
    {
        SetupInitialWave();

        // Sử dụng giá trị từ WaveData đầu tiên làm cooldown khởi động
        _waveCooldown = waves[_currentWaveIndex].timeBeforeNextWave;
        _isBetweenWaves = true;
    }


    void Update()
    {
        if (_isBetweenWaves)
        {
            _waveCooldown -= Time.deltaTime;
            if (_waveCooldown <= 0f)
            {
                // Logic kiểm tra điều kiện chiến thắng giữ nguyên...
                // LƯU Ý: Điều kiện chiến thắng vẫn phụ thuộc vào việc quái vật cuối cùng có bị tiêu diệt hay không.

                if (_waveCounter == 0) // Lần đầu tiên
                {
                    _waveCounter++;
                    OnWaveChanged?.Invoke(_waveCounter);

                    _isBetweenWaves = false;
                    _isSpawningGroup = true;
                }
                else // Các lần sau: Bắt đầu Wave mới ngay sau khi cooldown kết thúc
                {
                    StartNextWave();
                    _isBetweenWaves = false;
                }
            }
        }
        else
        {
            HandleWaveSpawning();

            // ĐÃ XÓA: Kiểm tra kết thúc Wave dựa trên _enemiesRemovedInWave
            // Logic chuyển trạng thái sang _isBetweenWaves đã được chuyển vào HandleWaveSpawning()
        }

        // Vẫn giữ lại kiểm tra điều kiện thắng để đảm bảo game kết thúc chính xác
        if (!_isSpawningGroup && _enemiesRemovedInWave >= _totalEnemiesInWave && _waveCounter >= LevelManager.Instance.CurrentLevel.wavesToWin && !_isEndlessMode)
        {
            OnMissionComplete?.Invoke();
        }
    }

    // HÀM MỚI: Thiết lập Wave đầu tiên (Wave 1)
    private void SetupInitialWave()
    {
        WaveData_Map5 initialWave = waves[_currentWaveIndex];
        _currentWaveGroups = new List<EnemyGroup_Map5>(initialWave.enemyGroups);
        _totalEnemiesInWave = initialWave.enemyGroups.Sum(group => group.enemiesCount);
        _enemiesSpawnedInWave = 0;
        _enemiesRemovedInWave = 0;
        _currentGroupIndex = 0;
        _enemiesSpawnedInCurrentGroup = 0;
        _isSpawningGroup = false;
    }


    // HÀM MỚI: Bắt đầu Wave tiếp theo (Từ Wave 2 trở đi)
    private void StartNextWave()
    {
        _currentWaveIndex = (_currentWaveIndex + 1);
        if (_currentWaveIndex >= waves.Length)
        {
            _currentWaveIndex = 0;
        }

        _waveCounter++;

        WaveData_Map5 nextWave = waves[_currentWaveIndex];
        _currentWaveGroups = new List<EnemyGroup_Map5>(nextWave.enemyGroups);

        _totalEnemiesInWave = nextWave.enemyGroups.Sum(group => group.enemiesCount);
        _enemiesSpawnedInWave = 0;
        _enemiesRemovedInWave = 0;

        _currentGroupIndex = 0;
        _enemiesSpawnedInCurrentGroup = 0;
        _isSpawningGroup = true;
        _groupSpawnTimer = 0f;

        OnWaveChanged?.Invoke(_waveCounter);

        if (_totalEnemiesInWave == 0)
        {
            // Nếu wave trống, vẫn phải tính cooldown cho wave kế tiếp
            _isBetweenWaves = true;
            _waveCooldown = nextWave.timeBeforeNextWave;
            _isSpawningGroup = false;
        }
    }

    // HÀM MỚI: Xử lý việc sinh quái theo từng Group
    private void HandleWaveSpawning()
    {
        if (_isSpawningGroup)
        {
            if (_currentGroupIndex >= _currentWaveGroups.Count)
            {
                _isSpawningGroup = false;

                // ⭐ THAY ĐỔI LỚN: CHUYỂN SANG TRẠNG THÁI NGHỈ NGAY SAU KHI SINH XONG ⭐
                WaveData_Map5 currentWave = waves[_currentWaveIndex];

                _isBetweenWaves = true;
                _waveCooldown = currentWave.timeBeforeNextWave; // Thiết lập thời gian chờ

                return;
            }

            EnemyGroup_Map5 currentGroup = _currentWaveGroups[_currentGroupIndex];

            _groupSpawnTimer -= Time.deltaTime;

            if (_groupSpawnTimer <= 0)
            {
                if (_enemiesSpawnedInCurrentGroup < currentGroup.enemiesCount)
                {
                    _groupSpawnTimer = currentGroup.spawnInterval;
                    SpawnEnemy(currentGroup);
                    _enemiesSpawnedInCurrentGroup++;
                    _enemiesSpawnedInWave++;
                }
                else
                {
                    _currentGroupIndex++;
                    _enemiesSpawnedInCurrentGroup = 0;
                    _groupSpawnTimer = 0f;
                }
            }
        }
    }


    // CẬP NHẬT HÀM SpawnEnemy để nhận EnemyGroup (Giữ nguyên)
    private void SpawnEnemy(EnemyGroup_Map5 group)
    {
        if (!_pathDictionary.TryGetValue(group.pathName, out var path))
        {
            Debug.LogError($"Path with name '{group.pathName}' not found in the scene! Check path name in WaveData.", gameObject);
            return;
        }

        if (_poolDictionary.TryGetValue(group.enemyType, out var pool))
        {
            GameObject spawnedObject = pool.GetPooledObject();

            spawnedObject.transform.position = path.GetPosition(0);

            float healthMultiplier = 1f + (_waveCounter * 0.1f);
            Bongma enemy = spawnedObject.GetComponent<Bongma>();
            enemy.SetPath(path);
            enemy.Initialize(healthMultiplier);

            spawnedObject.SetActive(true);
        }
        else
        {
            Debug.LogError($"Pool for enemy type {group.enemyType} not found in _poolDictionary!");
        }
    }

    // CẬP NHẬT LẠI CÁCH TÍNH ENEMY REMOVED (Giữ nguyên)
    private void HandleEnemyReachedEnd(EnemyData_Map5 data)
    {
        _enemiesRemovedInWave++;
    }

    private void HandleEnemyDestroyed(Bongma enemy)
    {
        _enemiesRemovedInWave++;
    }

    public void EnableEndlessMode()
    {
        _isEndlessMode = true;
    }

}