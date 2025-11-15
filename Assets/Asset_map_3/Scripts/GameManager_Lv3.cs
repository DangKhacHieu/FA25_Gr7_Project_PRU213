using System;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager_Lv3 : MonoBehaviour
{

    public static GameManager_Lv3 Instance {  get; private set; }
    // SỰ KIỆN MÀ UiController CẦN TÌM (NÓ ĐÃ BỊ MẤT TRƯỚC ĐÓ)
    public static event Action<int> OnLivesChanged;

    private int _lives = 1000;

    private float _gameSpeed = 1f;
    public float GameSpeed => _gameSpeed;
    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    [Header("UI Panels")]
    [SerializeField] private GameObject victoryPanel; // Kéo Panel "Chiến Thắng" vào đây
    [SerializeField] private GameObject gameOverPanel; // Kéo Panel "Thua Cuộc" vào đây

    private void OnEnable()
    {
        Enemy_Lv3.OnEnemyReachedEnd += HanldeEnemyReachesEnd;
        Spawner_Lv3.OnAllWavesCompleted += HandleAllWavesCompleted;
    }

    private void OnDisable()
    {
        Enemy_Lv3.OnEnemyReachedEnd -= HanldeEnemyReachesEnd;
        Spawner_Lv3.OnAllWavesCompleted -= HandleAllWavesCompleted;
    }

    private void Start()
    {
        // Ẩn cả 2 panel khi game bắt đầu
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        // Kích hoạt sự kiện lần đầu để UI cập nhật số máu
        _lives= LevelManager.Instance.CurrentLevel.startingLives;
        OnLivesChanged?.Invoke(_lives);

        // Đảm bảo thời gian chạy bình thường khi bắt đầu
        Time.timeScale = 1f;
    }

    private void HanldeEnemyReachesEnd(EnemyData_Lv3 data)
    {
        _lives = Mathf.Max(0, _lives - data.damage);
        OnLivesChanged?.Invoke(_lives);

        if (_lives <= 0)
        {
            Time.timeScale = 0f;
            Debug.Log("GAME OVER! Bạn đã hết máu.");
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }
        }
    }

    private void HandleAllWavesCompleted()
    {
        if (_lives > 0)
        {
            Time.timeScale = 0f;
            if (victoryPanel != null)
            {
                victoryPanel.SetActive(true);
            }
            Debug.Log("CHIẾN THẮNG!");
        }
        else
        {
            // Chế độ thường: Chiến thắng
            Time.timeScale = 0f;
            if (victoryPanel != null)
            {
                victoryPanel.SetActive(true);
            }
            Debug.Log("CHIẾN THẮNG!");
        }
    }

    public void RetryGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // for pausing/unpausing, UI needs
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

    // --- PHẦN THÊM VÀ CẬP NHẬT TRẠNG THÁI ---

    public void ResetGameState()
    {
        if (LevelManager.Instance == null || LevelManager.Instance.CurrentLevel == null)
        {
            Debug.LogError("LevelManager hoặc CurrentLevel không tồn tại khi cố gắng ResetGameState!");
            return;
        }

        // 1. Reset Máu
        _lives = LevelManager.Instance.CurrentLevel.startingLives;
        OnLivesChanged?.Invoke(_lives);

        // 2. Reset Vàng (Dùng CurrencyManager)
        if (CurrencyManager_Lv3.Instance != null)
        {
            CurrencyManager_Lv3.Instance.InitializeGold(
                LevelManager.Instance.CurrentLevel.startingResources);
        }
        else
        {
            Debug.LogError("CurrencyManager_Lv3 không tìm thấy! Không thể reset vàng.");
        }

        // 3. Reset Tốc độ
        SetGameSpeed(1f);

        // 4. Đảm bảo Panel ẩn
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Kích hoạt ResetGameState mỗi khi một scene mới được tải (ví dụ: Next Level)
        // Đặt TimeScale về 1f sau khi load level mới
        SetTimeScale(1f);
        ResetGameState();
    }
}


