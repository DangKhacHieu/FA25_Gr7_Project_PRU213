using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static event Action<int> OnLivesChanged;

    private int _lives = 1000;
    // --- THÊM MỚI ---
    [Header("UI Panels")]
    [SerializeField] private GameObject victoryPanel; // Kéo Panel "Chiến Thắng" vào đây
    [SerializeField] private GameObject gameOverPanel; // Kéo Panel "Thua Cuộc" vào đây


    private void OnEnable()
    {
        Enemy.OnEnemyReachedEnd += HanldeEnemyReachesEnd;
        // --- THÊM MỚI ---
        // Lắng nghe sự kiện "hết wave" từ Spawner
        Spawner.OnAllWavesCompleted += HandleAllWavesCompleted;
    }

    private void OnDisable()
    {
        Enemy.OnEnemyReachedEnd -= HanldeEnemyReachesEnd;
        // --- THÊM MỚI ---
        Spawner.OnAllWavesCompleted -= HandleAllWavesCompleted; 
    }

    private void Start()
    {

        // Ẩn cả 2 panel khi game bắt đầu
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        OnLivesChanged?.Invoke(_lives);

    }
    private void HanldeEnemyReachesEnd(EnemyData data)
    {
        _lives = Mathf.Max(0,_lives - data.damage);
        OnLivesChanged?.Invoke(_lives);

        // 3. THÊM MỚI: Kiểm tra nếu hết máu
        if (_lives <= 0)
        {
            // Dừng game ngay lập tức (đóng băng thời gian)
            Time.timeScale = 0f;

            // In ra Console để báo lỗi (tốt cho debug)
            Debug.Log("GAME OVER! Bạn đã hết máu.");

            // (Tùy chọn - Cách tốt hơn)
            // Thay vì chỉ dừng game, bạn nên hiện một màn hình Game Over
             if (gameOverPanel != null)
             {
                gameOverPanel.SetActive(true);
             }
        }
    }

    // --- HÀM MỚI ĐỂ XỬ LÝ CHIẾN THẮNG ---
    private void HandleAllWavesCompleted()
    {
        // Chỉ thắng nếu còn sống (máu > 0)
        if (_lives > 0)
        {
            Time.timeScale = 0f; // Dừng game
            if (victoryPanel != null)
            {
                victoryPanel.SetActive(true);
            }
            Debug.Log("CHIẾN THẮNG!");
        }
        // (Nếu _lives <= 0, màn hình "Game Over" đã được hiển thị rồi)
    }

    // --- HÀM MỚI ĐỂ CHƠI LẠI ---
    public void RetryGame()
    {

        // 1. Phục hồi thời gian về bình thường
        Time.timeScale = 1f;

        // 2. Tải lại màn chơi hiện tại
        // Lấy scene đang hoạt động và tải lại nó
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}


