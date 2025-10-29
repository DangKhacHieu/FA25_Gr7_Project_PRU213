using System.Collections.Generic;
using UnityEngine;
using System;

public class Spawner : MonoBehaviour
{

    public static event Action<int> OnWaveChange;

    [SerializeField] private Path[] paths; // nhiều đường đi

    [SerializeField] private WaveData[] waves;
    private int _currentWaveIndex = 0;
    private int _waveCounter = 0;
    private WaveData CurrentWave => waves[_currentWaveIndex];


    private float _spawTimer;
    // private float _spawnInterval = 1f;
    public GameObject Prefab;
    private float _spawnCounter;
    private int _enemiesRemoved;    

    [Header("Object Pools")]
    [SerializeField] private ObjectPooler Yetipool;
    [SerializeField] private ObjectPooler YetiTankerpool;
    [SerializeField] private ObjectPooler PhuThuyBangpool;
    [SerializeField] private ObjectPooler SnowManpool;
    [SerializeField] private ObjectPooler BossYetipool;

    private Dictionary<EnemyType, ObjectPooler> _poolDictionary;

    private float _timeBetweenWaves = 2f;
    private float _waveCooldown;
    private bool _isBetweenWaves = false;

    private void Awake()
    {
        _poolDictionary = new Dictionary<EnemyType, ObjectPooler>() 
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
        Enemy.OnEnemyReachedEnd += HandleEnemyReachedEnd;
    }

    private void OnDisable()
    {
        Enemy.OnEnemyReachedEnd -= HandleEnemyReachedEnd;
    }

    private void Start()
    {
        OnWaveChange?.Invoke(_currentWaveIndex);
    }

    void Update()
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
          
    }

    private void SpawnEnemy()
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

    }

    private void HandleEnemyReachedEnd(EnemyData data)
    {
        _enemiesRemoved++;
    }

}
