using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [Header("Tham chiếu UI")]
    public Button pauseButton;              // Nút pause ở góc màn hình
    public GameObject pausePanel;           // Panel hiển thị menu tạm dừng
    public Sprite playSprite;               // Icon khi game đang pause
    public Sprite pauseSprite;              // Icon khi game đang chạy

    private bool isPaused = false;

    void Start()
    {
        if (pauseButton != null)
            pauseButton.onClick.AddListener(TogglePause);

        if (pausePanel != null)
            pausePanel.SetActive(false); // ẩn panel khi bắt đầu
    }

    void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f; // Dừng game
            if (pauseButton != null) pauseButton.image.sprite = playSprite;
            if (pausePanel != null) pausePanel.SetActive(true);
            Debug.Log("⏸ Game paused – hiển thị PausePanel");
        }
        else
        {
            Time.timeScale = 1f; // Tiếp tục game
            if (pauseButton != null) pauseButton.image.sprite = pauseSprite;
            if (pausePanel != null) pausePanel.SetActive(false);
            Debug.Log("▶ Game resumed – ẩn PausePanel");
        }
    }

    // Cho phép resume từ nút Resume trong panel
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (pauseButton != null)
            pauseButton.image.sprite = pauseSprite;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        Debug.Log("▶ Resume game – đóng PausePanel");
    }


    public void QuitGame()
    {
        Application.Quit();
    }
    public void ResetPauseState()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (pauseButton != null)
            pauseButton.image.sprite = pauseSprite;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        Debug.Log("🔁 Reset pause state & icon");
    }

}
