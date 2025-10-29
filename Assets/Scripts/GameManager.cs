using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static event Action<int> OnLivesChanged;

    private int _lives = 3;

    private void OnEnable()
    {
        Enemy.OnEnemyReachedEnd += HanldeEnemyReachesEnd;
    }

    private void OnDisable()
    {
        Enemy.OnEnemyReachedEnd -= HanldeEnemyReachesEnd;
    }

    private void Start()
    {
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
            // if (gameOverPanel != null)
            // {
            //     gameOverPanel.SetActive(true);
            // }
        }
    }
}


