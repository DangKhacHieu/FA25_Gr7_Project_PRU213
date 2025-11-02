using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    // SỰ KIỆN MÀ UiController CẦN TÌM (NÓ ĐÃ BỊ MẤT TRƯỚC ĐÓ)
    public static event Action<int> OnLivesChanged;

    private int _lives = 1000;

    [Header("UI Panels")]
    [SerializeField] private GameObject victoryPanel; // Kéo Panel "Chiến Thắng" vào đây
    [SerializeField] private GameObject gameOverPanel; // Kéo Panel "Thua Cuộc" vào đây

    private void OnEnable()
    {
        Enemy.OnEnemyReachedEnd += HanldeEnemyReachesEnd;
        Spawner.OnAllWavesCompleted += HandleAllWavesCompleted;
    }

    private void OnDisable()
    {
        Enemy.OnEnemyReachedEnd -= HanldeEnemyReachesEnd;
        Spawner.OnAllWavesCompleted -= HandleAllWavesCompleted;
    }

    private void Start()
    {
        // Ẩn cả 2 panel khi game bắt đầu
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        // Kích hoạt sự kiện lần đầu để UI cập nhật số máu
        OnLivesChanged?.Invoke(_lives);

        // Đảm bảo thời gian chạy bình thường khi bắt đầu
        Time.timeScale = 1f;
    }

    private void HanldeEnemyReachesEnd(EnemyData data)
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
    }

    public void RetryGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}


