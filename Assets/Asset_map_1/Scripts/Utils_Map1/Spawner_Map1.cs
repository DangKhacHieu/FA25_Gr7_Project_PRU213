using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner_Map1 : MonoBehaviour
{
    public static Spawner_Map1 Instance { get; private set; }

    public static event Action<int> OnWaveChanged;
    public static event Action OnMissionComplete;

    [Header("Danh sách các Wave")]
    [SerializeField] private WaveData_Map1[] waves;

    [Header("Object Pools")]
    [SerializeField] private ObjectPooler_Map1 SlimePool;
    [SerializeField] private ObjectPooler_Map1 GobinPool;
    [SerializeField] private ObjectPooler_Map1 Slime_phan_noPool;
    [SerializeField] private ObjectPooler_Map1 Gobin_cung_thuPool;
    [SerializeField] private ObjectPooler_Map1 Slime_BossPool;

    private Dictionary<EnemyType_Map1, ObjectPooler_Map1> _poolDictionary;

    // _waveCounter là "Wave Index" (bắt đầu từ 0 = Wave 1)
    private int _waveCounter = 0;
    private int _activeEnemies = 0; // Số quái còn sống

    [Header("Thời gian giữa các wave")]
    //[SerializeField] private float _timeBetweenWaves = 5f;
    private float _waveCooldown;
    //private bool _isBetweenWaves = false;

    private bool _isEndlessMode = false;
    private bool _missionCompleted = false;

    private void Awake()
    {
        _poolDictionary = new Dictionary<EnemyType_Map1, ObjectPooler_Map1>()
        {
            { EnemyType_Map1.Slime, SlimePool },
            { EnemyType_Map1.Gobin, GobinPool },
            { EnemyType_Map1.Slime_phan_no, Slime_phan_noPool },
            { EnemyType_Map1.Gobin_cung_thu, Gobin_cung_thuPool },
            { EnemyType_Map1.Slime_Boss, Slime_BossPool }
        };

        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    private void OnEnable()
    {
        Enemy_Map1.OnEnemyDestroyed += HandleEnemyRemovedByDeath;
        Enemy_Map1.OnEnemyReachedEnd += HandleEnemyRemovedByReachEnd;
    }

    private void OnDisable()
    {
        Enemy_Map1.OnEnemyDestroyed -= HandleEnemyRemovedByDeath;
        Enemy_Map1.OnEnemyReachedEnd -= HandleEnemyRemovedByReachEnd;
    }

    private void Start()
    {
        OnWaveChanged?.Invoke(_waveCounter);
        StartCoroutine(StartWaveCoroutine());
    }

    // HÀM NÀY ĐÃ ĐƯỢC THAY ĐỔI LOGIC
    private IEnumerator StartWaveCoroutine()
    {
        while (true)
        {
            // 1. Lấy wave data
            WaveData_Map1 currentWave = waves[_waveCounter % waves.Length];

            // 2. Spawn wave
            yield return StartCoroutine(SpawnWave(currentWave));
            Debug.LogWarning($"ĐÃ SPAWN XONG WAVE {_waveCounter + 1}. BẮT ĐẦU ĐẾM NGƯỢC...");

            // 3. 🛑 THAY ĐỔI LOGIC: XÓA DÒNG CHỜ 🛑
            // Dòng 'yield return new WaitUntil...' đã bị xóa khỏi đây.

            // 4. 🛑 KIỂM TRA THẮNG (LOGIC MỚI) 🛑
            // Kiểm tra xem wave vừa spawn có phải là wave thắng không
            if ((_waveCounter + 1) >= LevelManager.Instance.CurrentLevel.wavesToWin && !_isEndlessMode)
            {
                // Nếu ĐÚNG (đã spawn wave cuối cùng)
                // Chúng ta ngừng vòng lặp, và đợi dọn dẹp quái
                Debug.LogWarning($"ĐÃ SPAWN WAVE CUỐI CÙNG ({_waveCounter + 1}). Đang chờ dọn dẹp {_activeEnemies} quái còn lại...");

                // CHỈ CHỜ Ở ĐÂY KHI ĐÃ SPAWN WAVE CUỐI
                yield return new WaitUntil(() => _activeEnemies == 0);

                // Khi giết hết, kích hoạt Mission Complete
                if (!_missionCompleted)
                {
                    _missionCompleted = true;
                    OnMissionComplete?.Invoke();
                    Debug.Log("MISSION COMPLETE! (Đã spawn đủ wave và giết hết quái)");
                }
                yield break; // Dừng coroutine này lại, không spawn nữa
            }

            // 5. Chờ cooldown (NẾU CHƯA THẮNG)
            //_isBetweenWaves = true;
            //_waveCooldown = _timeBetweenWaves;
            Debug.Log($"Wave tiếp theo ({_waveCounter + 2}) sẽ bắt đầu sau {_waveCooldown} giây...");
            yield return new WaitForSeconds(_waveCooldown);
           // _isBetweenWaves = false;

            // 6. Tăng wave index
            _waveCounter++;

            // 7. Cập nhật UI cho wave TIẾP THEO
            OnWaveChanged?.Invoke(_waveCounter);
        }
    }

    private IEnumerator SpawnWave(WaveData_Map1 wave)
    {
        yield return new WaitForSeconds(wave.startDelay);

        foreach (var group in wave.enemyGroups)
        {
            if (!_poolDictionary.TryGetValue(group.enemyType, out var pool))
                continue;

            Path_Map1 path = group.targetPath;
            if (path == null && !string.IsNullOrEmpty(group.pathName))
            {
                var found = GameObject.Find(group.pathName);
                if (found) path = found.GetComponent<Path_Map1>();
            }

            if (path == null)
            {
                Debug.LogError($"Wave '{wave.waveName}' nhóm {group.enemyType} không có Path hợp lệ!");
                continue;
            }

            for (int i = 0; i < group.count; i++)
            {
                GameObject enemyObj = pool.GetPooledObject();
                if (enemyObj == null) continue;

                Enemy_Map1 enemy = enemyObj.GetComponent<Enemy_Map1>();
                if (enemy != null)
                {
                    enemyObj.SetActive(true);
                    enemy.SetPath(path);
                    enemy.Initialize(1f + (_waveCounter * 0.1f));
                    _activeEnemies++;
                }

                yield return new WaitForSeconds(group.spawnInterval);
            }
        }
    }

    private void HandleEnemyRemovedByDeath(Enemy_Map1 enemy)
    {
        _activeEnemies = Mathf.Max(0, _activeEnemies - 1);
        CheckWaveClear();
    }

    private void HandleEnemyRemovedByReachEnd(EnemyData_Map1 data)
    {
        _activeEnemies = Mathf.Max(0, _activeEnemies - 1);
        CheckWaveClear();
    }

    // Hàm này không còn dùng để check logic wave nữa, chỉ để debug
    private void CheckWaveClear()
    {
        // Debug.Log($"Số quái còn lại: {_activeEnemies}");
    }

    public void RegisterSpawnedEnemy()
    {
        _activeEnemies++;
        Debug.Log($"Boss đã đăng ký 1 lính mới. Tổng quái: {_activeEnemies}");
    }

    public void EnableEndlessMode()
    {
        if (_isEndlessMode) return;

        _isEndlessMode = true;
        _missionCompleted = false;
        Debug.Log("♾️ Endless Mode Activated!");

        // Bắt đầu lại Coroutine (nó đã bị 'yield break' ở trên)
        StartCoroutine(StartEndlessWaves());
    }

    // HÀM NÀY CŨNG ĐÃ ĐƯỢC THAY ĐỔI LOGIC
    private IEnumerator StartEndlessWaves()
    {
        // Vòng lặp này sẽ chạy mãi mãi
        while (true)
        {
            // 1. Lấy wave data
            WaveData_Map1 currentWave = waves[_waveCounter % waves.Length];

            // 2. Cập nhật UI (UI đã được cập nhật ở vòng lặp trước rồi)
            Debug.Log($"♾️ ENDLESS: BẮT ĐẦU SPAWN WAVE {_waveCounter + 1}.");

            // 3. Spawn wave
            yield return StartCoroutine(SpawnWave(currentWave));
            Debug.LogWarning($"♾️ ENDLESS: ĐÃ SPAWN XONG WAVE {_waveCounter + 1}. BẮT ĐẦU ĐẾM NGƯỢC...");

            // 4. 🛑 THAY ĐỔI LOGIC: KHÔNG CHỜ GIẾT HẾT QUÁI 🛑
            // yield return new WaitUntil(() => _activeEnemies == 0); // <--- ĐÃ XÓA

            // 5. Chờ cooldown
            //_isBetweenWaves = true;
            //_waveCooldown = _timeBetweenWaves;
            Debug.Log($"♾️ ENDLESS: Wave tiếp theo ({_waveCounter + 2}) sẽ bắt đầu sau {_waveCooldown} giây...");
            yield return new WaitForSeconds(_waveCooldown);
            //_isBetweenWaves = false;

            // 6. Tăng wave và cập nhật UI
            _waveCounter++;
            OnWaveChanged?.Invoke(_waveCounter);
        }
    }
}