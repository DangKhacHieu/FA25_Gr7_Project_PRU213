using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.EventSystems.EventTrigger;

public class GameManager_Map5 : MonoBehaviour
{

    public static GameManager_Map5 Instance { get; private set; }


    public static event Action<int> OnLivesChanged;
    public static event Action<int> OnResourcesChanged;

    private int _lives = 5;
    private int _resources = 175;
    public int Resources => _resources;

    private float _gameSpeed = 1f;
    public float GameSpeed => _gameSpeed;


    private void Awake()
    {
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
        SceneManager.sceneLoaded += OnSceneLoaded;

    }

    private void OnDisable()
    {
        Bongma.OnEnemyReachedEnd -= HandleEnemyReachedEnd;
        Bongma.OnEnemyDestroyed -= HandleEnemyDestroyed;
        SceneManager.sceneLoaded -= OnSceneLoaded;

    }

    private void Start()
    {
        OnLivesChanged?.Invoke(_lives);
        OnResourcesChanged?.Invoke(_resources);
    }

    private void HandleEnemyReachedEnd(EnemyData_Map5 data)
    {
        _lives = Mathf.Max(0, _lives - data.damage);
        OnLivesChanged?.Invoke(_lives);
    }

    private void HandleEnemyDestroyed(Bongma enemy)
    {
        AddResources(Mathf.RoundToInt(enemy.Data.resourceReward));
    }

    private void AddResources(int amount)
    {
        _resources += amount;
        OnResourcesChanged?.Invoke(_resources);
    }

    public void SetTimeScale(float scale)
    {
        Time.timeScale = scale;
    }

    // for game speed buttons
    public void SetGameSpeed(float newSpeed)
    {
        _gameSpeed = newSpeed;
        SetTimeScale(_gameSpeed);
    }


    public void SpendResources(int amount)
    {
        if (_resources >= amount)
        {
            _resources -= amount;
            OnResourcesChanged?.Invoke(_resources);
        }
    }

    public void ResetGameState()
    {
        _lives = LevelManager.Instance.CurrentLevel.startingLives;
        OnLivesChanged?.Invoke(_lives);
        _resources = LevelManager.Instance.CurrentLevel.startingResources;
        OnResourcesChanged?.Invoke(_resources);

        SetGameSpeed(1f);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetGameState();
    }


}
