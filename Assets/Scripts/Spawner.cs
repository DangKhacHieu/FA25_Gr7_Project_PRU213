using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyPoolMapping
{
    public EnemyType enemyType;
    public ObjectPooler pool;
}

public class Spawner : MonoBehaviour
{
    public static Spawner Instance { get; private set; }

    public static event Action<int> OnWaveChanged;
    public static event Action OnMissionComplete;

    [Header("Wave Settings")]
    [SerializeField] private WaveData[] waves;
    private int _currentWaveIndex = 0;
    private int _waveCounter = 0;
    private WaveData CurrentWave => waves[_currentWaveIndex];

    private int _enemiesRemoved;
    private bool _isBetweenWaves = false;
    private bool _isEndlessMode = false;

    [Header("Enemy Pools")]
    [SerializeField] private List<EnemyPoolMapping> enemyPools = new List<EnemyPoolMapping>();
    private Dictionary<EnemyType, ObjectPooler> _poolDictionary;

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

        // Khởi tạo dictionary pool
        _poolDictionary = new Dictionary<EnemyType, ObjectPooler>();
        foreach (var mapping in enemyPools)
        {
            if (!_poolDictionary.ContainsKey(mapping.enemyType))
                _poolDictionary.Add(mapping.enemyType, mapping.pool);
        }

        // Tự động tìm PathManage nếu chưa được gán
        if (pathManagerRoot == null)
            pathManagerRoot = GameObject.Find("PathManage")?.transform;
    }

    private void OnEnable()
    {
        Enemy.OnEnemyReachedEnd += HandleEnemyRemoved;
        Enemy.OnEnemyDestroyed += HandleEnemyRemoved;
    }

    private void OnDisable()
    {
        Enemy.OnEnemyReachedEnd -= HandleEnemyRemoved;
        Enemy.OnEnemyDestroyed -= HandleEnemyRemoved;
    }

    private void Start()
    {
        StartCoroutine(RunWaveRoutine());
    }

    private IEnumerator RunWaveRoutine()
    {
        yield return new WaitForSeconds(2f); // đợi game load
        while (_currentWaveIndex < waves.Length || _isEndlessMode)
        {
            if (_isEndlessMode)
            {
                Debug.Log("_isEndlessMode");
            }
            WaveData wave = CurrentWave;
            _waveCounter++;
            OnWaveChanged?.Invoke(_waveCounter);

            _enemiesRemoved = 0;
            int totalEnemiesThisWave = CountEnemiesInWave(wave);

            Debug.Log($"[Spawner] 🚀 Bắt đầu Wave {_waveCounter}: tổng {totalEnemiesThisWave} quái");

            // Chạy song song các nhóm trong wave
            foreach (SubWaveData sub in wave.subWaves)
            {
                StartCoroutine(SpawnSubWave(sub));
            }

            // Đợi đến khi toàn bộ enemy của wave bị tiêu diệt
            yield return new WaitUntil(() => _enemiesRemoved >= totalEnemiesThisWave);

            Debug.Log($"[Spawner] ✅ Hoàn thành Wave {_waveCounter}");

            // Nếu là wave cuối và không ở endless mode -> thắng
            if (_currentWaveIndex + 1 >= waves.Length && !_isEndlessMode)
            {
                OnMissionComplete?.Invoke();
                yield break;
            }

            // Nghỉ giữa 2 wave
            _isBetweenWaves = true;
            _waveCooldown = _timeBetweenWaves;
            yield return new WaitForSeconds(_waveCooldown);
            _isBetweenWaves = false;

            _currentWaveIndex = (_currentWaveIndex + 1) % waves.Length;
        }
    }

    private IEnumerator SpawnSubWave(SubWaveData sub)
    {
        yield return new WaitForSeconds(sub.startDelay);

        // Tìm pool tương ứng enemy type
        if (!_poolDictionary.TryGetValue(sub.enemyType, out var pool))
        {
            Debug.LogWarning($"⚠️ Không tìm thấy pool cho enemy type: {sub.enemyType}");
            yield break;
        }

        // ✅ 1️⃣ Fallback: tìm Path theo tên trong PathManager
        Path assignedPath = null;
        if (pathManagerRoot != null)
        {
            var found = pathManagerRoot.Find(sub.pathName);
            assignedPath = found ? found.GetComponent<Path>() : null;
        }

        // ✅ 2️⃣ Nếu vẫn null -> báo lỗi và bỏ qua nhóm này
        if (assignedPath == null)
        {
            Debug.LogWarning($"⚠️ SubWave {sub.enemyType} chưa được gán Path (hoặc sai tên)! PathName: {sub.pathName}");
            yield break;
        }

        // ✅ 3️⃣ Spawn enemy theo path
        for (int i = 0; i < sub.enemyCount; i++)
        {
            GameObject obj = pool.GetPooledObject();
            obj.transform.position = assignedPath.GetPosition(0);

            Enemy enemy = obj.GetComponent<Enemy>();
            enemy.Initialize(sub.healthMultiplier, assignedPath);
            obj.SetActive(true);

            yield return new WaitForSeconds(sub.spawnInterval);
        }
    }

    private void HandleEnemyRemoved(Enemy enemy)
    {
        _enemiesRemoved++;
    }

    private void HandleEnemyRemoved(EnemyData data)
    {
        _enemiesRemoved++;
    }

    private int CountEnemiesInWave(WaveData wave)
    {
        int total = 0;
        foreach (var sub in wave.subWaves)
            total += sub.enemyCount;
        return total;
    }

    public void EnableEndlessMode()
    {
        _isEndlessMode = true;
        StartCoroutine(RunWaveRoutine());
    }
}
