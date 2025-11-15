using UnityEngine;
using UnityEngine.UI;

public class UI_PauseGameController : MonoBehaviour
{
    [Header("Panel hiển thị khi tạm dừng")]
    public GameObject pausePanel;

    [Header("Nút đóng panel (Resume)")]
    public Button closeButton;

    private bool isPaused = false;

    void Start()
    {
        // Ẩn panel khi bắt đầu game
        if (pausePanel != null)
            pausePanel.SetActive(false);

        // Gán sự kiện cho nút close nếu có
        if (closeButton != null)
            closeButton.onClick.AddListener(HidePausePanel);
    }

    /// <summary>
    /// Hiện panel tạm dừng (pause game)
    /// </summary>
    public void ShowPausePanel()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
            Time.timeScale = 0f;
            isPaused = true;
            Debug.Log("⏸ Game paused – hiển thị PausePanel");
        }
    }

    /// <summary>
    /// Ẩn panel (resume game)
    /// </summary>
    public void HidePausePanel()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1f;
            isPaused = false;
            Debug.Log("▶ Game resumed – ẩn PausePanel");
        }
    }

    /// <summary>
    /// Bật/tắt panel bằng cùng 1 nút (Pause/Resume)
    /// </summary>
    public void TogglePause()
    {
        if (isPaused)
            HidePausePanel();
        else
            ShowPausePanel();
    }
}
