using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Spawner : MonoBehaviour
{
    // --- THÊM DÒNG NÀY VÀO ĐÂY ---
    // Sự kiện này sẽ thông báo cho CurrencyManager biết wave nào vừa xong
    public static event Action<int> OnWaveCompleted;
    // --- HẾT THÊM MỚI ---

    public static event Action<int> OnWaveChange;

    [SerializeField] private Path[] paths; // nhiều đường đi

    [SerializeField] private WaveData[] waves;

    // code mới thêm 
    [SerializeField] private float timeBetweenWaves = 5f; // Thời gian chờ giữa các wave

    // --- THÊM DÒNG NÀY ---
    public static event Action OnAllWavesCompleted;
    // --- HẾT PHẦN THÊM MỚI ---

    private int _currentWaveIndex = 0;
    private int _waveCounter = 1;
    private WaveData CurrentWave => waves[_currentWaveIndex];

    // Đếm quái để biết khi nào wave kết thúc
    private int _enemiesSpawned;
    private int _enemiesRemoved; // (Bạn đã có _enemiesRemoved)

   // private float _spawTimer;
    // private float _spawnInterval = 1f;
  /*  public GameObject Prefab;
    private float _spawnCounter;
    private int _enemiesRemoved;    */

    [Header("Object Pools")]
    [SerializeField] private ObjectPooler Yetipool;
    [SerializeField] private ObjectPooler YetiTankerpool;
    [SerializeField] private ObjectPooler PhuThuyBangpool;
    [SerializeField] private ObjectPooler SnowManpool;
    [SerializeField] private ObjectPooler BossYetipool;

    public static Dictionary<EnemyType, ObjectPooler> PoolDictionary { get; private set; }

    /*private float _timeBetweenWaves = 2f;
    private float _waveCooldown;
    private bool _isBetweenWaves = false;*/

    private void Awake()
    {
        PoolDictionary = new Dictionary<EnemyType, ObjectPooler>() 
        {
            { EnemyType.yeti,Yetipool},
            { EnemyType.YetiTanker,YetiTankerpool},
            { EnemyType.PhuThuyBang,PhuThuyBangpool},
            { EnemyType.SnowMan,SnowManpool},
            { EnemyType.BossYeti,BossYetipool},
        };

    }

    private void OnEnable()
    {
        // Giữ nguyên
        Enemy.OnEnemyReachedEnd += HandleEnemyReachedEnd;
        // Bạn cũng cần một event khi quái chết (Enemy.OnEnemyDied)
        // Enemy.OnEnemyDied += HandleEnemyRemoved;
        // Vì hiện tại bạn chỉ đếm quái về đích, nếu quái bị giết,
        // wave sẽ không bao giờ kết thúc.
        // Tạm thời, tôi sẽ giả định bạn thêm event OnEnemyDied

        // --- THÊM DÒNG NÀY ---
        Enemy.OnEnemyDied += HandleEnemyDied;
    }

    private void OnDisable()
    {
        // Giữ nguyên
        Enemy.OnEnemyReachedEnd -= HandleEnemyReachedEnd;
        // Enemy.OnEnemyDied -= HandleEnemyRemoved;

        // --- THÊM DÒNG NÀY ---
        Enemy.OnEnemyDied -= HandleEnemyDied;
    }

    private void Start()
    {
        //  OnWaveChange?.Invoke(_currentWaveIndex);
        // Bắt đầu vòng lặp wave
        StartCoroutine(SpawnWaves());
    }

    /* void Update()
     {
         if (_isBetweenWaves)
         {
             _waveCooldown -= Time.deltaTime;
                 if(_waveCooldown <= 0f)
             {
                 _currentWaveIndex = (_currentWaveIndex + 1) % waves.Length;
                 _waveCounter++;
                 OnWaveChange?.Invoke(_waveCounter);
                 _spawnCounter = 0;
                 _enemiesRemoved = 0;
                 _spawTimer = 0f;
                 _isBetweenWaves = false;
             }
         }
         else
         {
             _spawTimer -= Time.deltaTime;
             if (_spawTimer <= 0 && _spawnCounter < CurrentWave.enemiesPerWave)
             {
                 _spawTimer = CurrentWave.spawnInterval;
                 SpawnEnemy();
                 _spawnCounter++;
             }
             else if (_spawnCounter >= CurrentWave.enemiesPerWave && _enemiesRemoved >=
                 CurrentWave.enemiesPerWave)
             {
                 _isBetweenWaves = true;
                 _waveCooldown = _timeBetweenWaves;

             }
         }

     }*/
    // mới thêm
    private IEnumerator SpawnWaves()
    {
        while (true) // Lặp vô hạn các wave (hoặc bạn có thể thay đổi)
        {
            if (_currentWaveIndex >= waves.Length)
            {
                Debug.Log("Đã hoàn thành tất cả các wave!");
                // TODO: Xử lý logic thắng game
                OnAllWavesCompleted?.Invoke(); // Thông báo cho cả game biết đã thắng
                yield break; // Dừng coroutine
            }

            OnWaveChange?.Invoke(_waveCounter); // Thông báo UI (Wave 1, 2, 3...)

            WaveData currentWave = waves[_currentWaveIndex];

            // Reset bộ đếm cho wave mới
            _enemiesSpawned = 0;
            _enemiesRemoved = 0;

            // Bắt đầu spawn từng nhóm trong wave
            foreach (EnemyGroup group in currentWave.enemyGroups)
            {
                StartCoroutine(SpawnEnemyGroup(group));
            }

            // Chờ cho đến khi tất cả quái đã spawn VÀ tất cả quái đã bị xóa
            // (về đích HOẶC bị giết)
            yield return new WaitUntil(() => _enemiesRemoved >= _enemiesSpawned);

            Debug.Log($"Hoàn thành Wave: {_waveCounter}");

            // --- THÊM MỚI ĐỂ GỌI CurrencyManager ---
            //
            // Bắn tín hiệu cho CurrencyManager biết wave này đã xong.
            // Vì _waveCounter bắt đầu từ 1, nó sẽ gửi đi 1, 2, 3...
            // CurrencyManager sẽ nhận và kiểm tra (switch) xem có phải 3, 5, 8 không.
            //
            OnWaveCompleted?.Invoke(_waveCounter);
            // --- HẾT THÊM MỚI ---

            // TODO: Thưởng vàng cho wave
            // GameManager.Instance.AddGold(currentWave.waveGoldReward);

            // Chờ giữa các wave
            yield return new WaitForSeconds(timeBetweenWaves);

            // Chuyển sang wave tiếp theo
            _currentWaveIndex++;
            _waveCounter++;
        }
    }

    private IEnumerator SpawnEnemyGroup(EnemyGroup group)
    {
        // 1. Chờ delay của nhóm (nếu có)
        if (group.delayBeforeGroup > 0)
        {
            yield return new WaitForSeconds(group.delayBeforeGroup);
        }

        // 2. Spawn quái trong nhóm
        for (int i = 0; i < group.count; i++)
        {
            // Tăng bộ đếm tổng số quái đã spawn
            _enemiesSpawned++;

            // Gọi hàm SpawnEnemy của bạn
            SpawnEnemy(group.enemyType);

            // Chờ theo thời gian giãn cách của nhóm
            yield return new WaitForSeconds(group.spawnInterval);
        }
    }
    // mới thêm 
    // Hàm SpawnEnemy của bạn được sửa đổi một chút để nhận tham số
    private void SpawnEnemy(EnemyType enemyTypeToSpawn)
    {
        if (PoolDictionary.TryGetValue(enemyTypeToSpawn, out var pool))
        {
            GameObject spawnedObject = pool.GetPoolObject();

            // Logic chọn path ngẫu nhiên của bạn (giữ nguyên)
            Path chosenPath = paths[UnityEngine.Random.Range(0, paths.Length)];

            spawnedObject.transform.position = chosenPath.GetPosition(0);

            Enemy enemy = spawnedObject.GetComponent<Enemy>();
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

    /*private void SpawnEnemy()
    {
        if (_poolDictionary.TryGetValue(CurrentWave.enemyTpye, out var pool))
        {
            GameObject spawnedObject = pool.GetPoolObject();
            //spawnedObject.transform.position = transform.position;

            // Chọn ngẫu nhiên 1 đường đi (hoặc tùy wave)
            Path chosenPath = paths[UnityEngine.Random.Range(0, paths.Length)];

            // Spawn ở waypoint đầu tiên của path đó
            spawnedObject.transform.position = chosenPath.GetPosition(0);

            // 🧩 Gán path vào Enemy
            Enemy enemy = spawnedObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.SetPath(chosenPath);
            }
            spawnedObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning($"Không tìm thấy pool cho loại quái: {CurrentWave.enemyTpye}");
        }

    }*/

    private void HandleEnemyReachedEnd(EnemyData data)
    {
        _enemiesRemoved++;
    }

    private void HandleEnemyDied(EnemyData data)
    {
        _enemiesRemoved++;
    }
}
