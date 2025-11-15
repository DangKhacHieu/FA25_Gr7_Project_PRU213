using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.UIElements; // Đã loại bỏ using không cần thiết

public class Spawner_Lv3 : MonoBehaviour
{
    // Thêm biến cờ trạng thái Endless Mode
    private bool _isEndless = false;
    public static Spawner_Lv3 Instance { get; private set; }

    // --- EVENTS ---
    public static event Action<int> OnWaveCompleted;
    public static event Action<int> OnWaveChange;
    public static event Action OnAllWavesCompleted; // Thắng game (chế độ thường)

    // --- CONFIG ---
    [SerializeField] private Path_Lv3[] paths;
    [SerializeField] private WaveData_Lv3[] waves;
    [SerializeField] private float timeBetweenWaves = 5f;

    // --- STATE ---
    private int _currentWaveIndex = 0;
    private int _waveCounter = 1;
    private WaveData_Lv3 CurrentWave => waves[_currentWaveIndex];

    private int _enemiesSpawned;
    private int _enemiesRemoved;

    // ---- THÊM BIẾN NÀY ----
    // Biến này theo dõi đường đi tiếp theo sẽ được dùng
    private int _nextPathIndex = 0;
    // -----------------------

    [Header("Object Pools")]
    [SerializeField] private ObjectPooler_Lv3 Yetipool;
    [SerializeField] private ObjectPooler_Lv3 YetiTankerpool;
    [SerializeField] private ObjectPooler_Lv3 PhuThuyBangpool;
    [SerializeField] private ObjectPooler_Lv3 SnowManpool;
    [SerializeField] private ObjectPooler_Lv3 BossYetipool;

    public static Dictionary<EnemyType_Lv3, ObjectPooler_Lv3> PoolDictionary { get; private set; }

    private void Awake()
    {
        // Logic Singleton cục bộ
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        // Khởi tạo Pool Dictionary
        PoolDictionary = new Dictionary<EnemyType_Lv3, ObjectPooler_Lv3>()
        {
            { EnemyType_Lv3.yeti,Yetipool},
            { EnemyType_Lv3.YetiTanker,YetiTankerpool},
            { EnemyType_Lv3.PhuThuyBang,PhuThuyBangpool},
            { EnemyType_Lv3.SnowMan,SnowManpool},
            { EnemyType_Lv3.BossYeti,BossYetipool},
        };

    }

    private void OnEnable()
    {
        Enemy_Lv3.OnEnemyReachedEnd += HandleEnemyReachedEnd;
        Enemy_Lv3.OnEnemyDied += HandleEnemyDied;

    }

    private void OnDisable()
    {
        Enemy_Lv3.OnEnemyReachedEnd -= HandleEnemyReachedEnd;
        Enemy_Lv3.OnEnemyDied -= HandleEnemyDied;
    }

    private void Start()
    {
        // Cần kiểm tra nếu level là Endless Mode ngay từ đầu
        if (LevelManager.Instance != null && LevelManager.Instance.CurrentLevel != null)
        {
            if (LevelManager.Instance.CurrentLevel.wavesToWin <= 0 )
                //LevelManager.Instance.CurrentLevel.isEndlessMode)
            {
                _isEndless = true;
                Debug.Log("Spawner: Khởi động ở chế độ Endless Mode!");
            }
        }
        StartCoroutine(SpawnWaves());
    }

    /*private IEnumerator SpawnWaves()
    {
        while (true) // Lặp vô hạn
        {
            // Kiểm tra LevelManager (chỉ cần một lần ở đầu loop)
            if (LevelManager.Instance == null || LevelManager.Instance.CurrentLevel == null)
            {
                Debug.LogError("LevelManager hoặc CurrentLevel không được thiết lập!");
                yield break;
            }

            var currentLevel = LevelManager.Instance.CurrentLevel;

            // --- 1. KIỂM TRA ĐIỀU KIỆN THẮNG (CHẾ ĐỘ THƯỜNG) ---
            // Nếu không phải Endless MODE VÀ đã hoàn thành SỐ wave cần để thắng: DỪNG.
            if (!_isEndless &&
                currentLevel.wavesToWin > 0 &&
                _waveCounter > currentLevel.wavesToWin)
            {
                Debug.Log("Đã hoàn thành tất cả các wave đã định sẵn, CHIẾN THẮNG!");
                OnAllWavesCompleted?.Invoke();
                yield break; // Dừng coroutine
            }

            // --- 2. XỬ LÝ LẶP WAVE (ENDLESS MODE VÀ CHẾ ĐỘ THƯỜNG) ---
            if (_currentWaveIndex >= waves.Length)
            {
                // Nếu hết wave định sẵn trong mảng (waves[])
                if (_isEndless || currentLevel.wavesToWin <= 0)
                {
                    Debug.Log($"Endless Mode: Bắt đầu Vòng lặp Wave {_waveCounter / waves.Length + 1}!");
                    _currentWaveIndex = 0; // Reset index để lặp lại waves[]
                    // TODO: Gọi hàm tăng độ khó (nếu có)
                    // IncreaseDifficulty(_waveCounter); 
                }
                else
                {
                    // Đã chạy hết mảng waves[] nhưng chưa đạt wavesToWin -> Lỗi cấu hình.
                    Debug.LogError("Lỗi cấu hình: Mảng waves[] quá ngắn so với wavesToWin.");
                    OnAllWavesCompleted?.Invoke();
                    yield break;
                }
            }

            OnWaveChange?.Invoke(_waveCounter);

            WaveData_Lv3 currentWave = waves[_currentWaveIndex];

            // Reset bộ đếm cho wave mới
            _enemiesSpawned = 0;
            _enemiesRemoved = 0;

            // Bắt đầu spawn
            foreach (EnemyGroup_LV3 group in currentWave.enemyGroups)
            {
                StartCoroutine(SpawnEnemyGroup(group));
            }

            // Chờ wave kết thúc
            yield return new WaitUntil(() => _enemiesRemoved >= _enemiesSpawned);

            // --- 3. KẾT THÚC WAVE ---
            Debug.Log($"Hoàn thành Wave: {_waveCounter}");
            OnWaveCompleted?.Invoke(_waveCounter);

            yield return new WaitForSeconds(timeBetweenWaves);

            // Chuyển sang wave tiếp theo
            _currentWaveIndex++;
            _waveCounter++;
        }
    }*/

    private IEnumerator SpawnWaves()
    {
        while (true) // Lặp vô hạn
        {
            if (LevelManager.Instance == null || LevelManager.Instance.CurrentLevel == null)
            {
                Debug.LogError("LevelManager hoặc CurrentLevel không được thiết lập!");
                yield break;
            }

            var currentLevel = LevelManager.Instance.CurrentLevel;

            // --- 1. KIỂM TRA ĐIỀU KIỆN THẮNG (CHẾ ĐỘ THƯỜNG) ---
            if (!_isEndless &&
                currentLevel.wavesToWin > 0 &&
                _waveCounter > currentLevel.wavesToWin)
            {
                Debug.Log("Đã hoàn thành tất cả các wave đã định sẵn, CHIẾN THẮNG!");
                OnAllWavesCompleted?.Invoke(); // Hiển thị panel "Mission Complete" (và dừng Time.timeScale)

                // ---- THAY ĐỔI QUAN TRỌNG NHẤT ----
                // Dùng vòng lặp 'while' thay vì 'WaitUntil'.
                // 'yield return null' sẽ tiếp tục chạy mỗi frame ngay cả khi Time.timeScale = 0
                Debug.Log("Spawner: Đang tạm dừng, chờ người chơi chọn Endless Mode...");
                while (_isEndless == false)
                {
                    yield return null; // Chờ 1 frame rồi kiểm tra lại
                }

                // Khi _isEndless == true, vòng lặp 'while' kết thúc và code chạy tiếp
                Debug.Log("Spawner: Đã tiếp tục, Endless Mode đang chạy!");
                // -------------------------------------
            }

            // --- 2. XỬ LÝ LẶP WAVE (ENDLESS MODE VÀ CHẾ ĐỘ THƯỜNG) ---
            if (_currentWaveIndex >= waves.Length)
            {
                if (_isEndless || currentLevel.wavesToWin <= 0)
                {
                    Debug.Log($"Endless Mode: Bắt đầu Vòng lặp Wave {_waveCounter / waves.Length + 1}!");
                    _currentWaveIndex = 0;
                }
                else
                {
                    Debug.LogError("Lỗi cấu hình: Mảng waves[] quá ngắn so với wavesToWin.");
                    OnAllWavesCompleted?.Invoke();
                    yield break;
                }
            }

            OnWaveChange?.Invoke(_waveCounter);

            WaveData_Lv3 currentWave = waves[_currentWaveIndex];

            _enemiesSpawned = 0;
            _enemiesRemoved = 0;

            foreach (EnemyGroup_LV3 group in currentWave.enemyGroups)
            {
                StartCoroutine(SpawnEnemyGroup(group));
            }

            yield return new WaitUntil(() => _enemiesRemoved >= _enemiesSpawned);

            // --- 3. KẾT THÚC WAVE ---
            Debug.Log($"Hoàn thành Wave: {_waveCounter}");
            OnWaveCompleted?.Invoke(_waveCounter);

            yield return new WaitForSeconds(timeBetweenWaves);

            _currentWaveIndex++;
            _waveCounter++;
        }
    }

    // ... (Giữ nguyên SpawnEnemyGroup và SpawnEnemy) ...

    private IEnumerator SpawnEnemyGroup(EnemyGroup_LV3 group)
    {
        // ... (Giữ nguyên logic)
        if (group.delayBeforeGroup > 0)
        {
            yield return new WaitForSeconds(group.delayBeforeGroup);
        }

        for (int i = 0; i < group.count; i++)
        {
            _enemiesSpawned++;
            SpawnEnemy(group.enemyType);
            yield return new WaitForSeconds(group.spawnInterval);
        }
    }

    private void SpawnEnemy(EnemyType_Lv3 enemyTypeToSpawn)
    {
        // ... (Giữ nguyên logic TryGetValue)
        if (PoolDictionary.TryGetValue(enemyTypeToSpawn, out var pool))
        {
            GameObject spawnedObject = pool.GetPoolObject();

            // ---- LOGIC MỚI ĐỂ CHỌN ĐƯỜNG CÂN BẰNG ----

            // 1. Chọn đường đi dựa trên index hiện tại
            // Đảm bảo rằng mảng 'paths' không bị rỗng
            if (paths.Length == 0)
            {
                Debug.LogError("Mảng 'paths' bị rỗng! Không thể spawn quái.");
                return; // Thoát ra nếu không có đường đi
            }

            Path_Lv3 chosenPath = paths[_nextPathIndex];

            // 2. Tăng index lên cho lần spawn tiếp theo
            _nextPathIndex++;

            // 3. Nếu index vượt quá số lượng đường, quay về 0
            if (_nextPathIndex >= paths.Length)
            {
                _nextPathIndex = 0;
            }
            // -----------------------------------------

            spawnedObject.transform.position = chosenPath.GetPosition(0);
            Enemy_Lv3 enemy = spawnedObject.GetComponent<Enemy_Lv3>();
            if (enemy != null)
            {
                enemy.SetPath(chosenPath);
            }
            spawnedObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning($"Không tìm thấy pool cho loại quái: {enemyTypeToSpawn}");
        }
    }

    private void HandleEnemyReachedEnd(EnemyData_Lv3 data)
    {
        _enemiesRemoved++;
    }

    private void HandleEnemyDied(EnemyData_Lv3 data)
    {
        _enemiesRemoved++;
    }

    // HÀM PUBLIC ĐƯỢC GỌI TỪ UI/GAMEMANAGER ĐỂ CHUYỂN SANG ENDLESS MODE
    public void EnableEndlessMode()
    {
        // Kích hoạt cờ Endless Mode, ghi đè quy tắc wavesToWin
        _isEndless = true;

        // Đặt lại wave index về 0 để đảm bảo vòng lặp wave bắt đầu lại (nếu nó đã dừng ở cuối mảng)
        // Lưu ý: Coroutine SpawnWaves() sẽ không bị dừng nếu nó đang chạy.
        // Nó chỉ tiếp tục lặp khi wave hiện tại kết thúc.

        Debug.Log("Endless Mode Activated! Spawner will now loop waves indefinitely.");
    }
}
