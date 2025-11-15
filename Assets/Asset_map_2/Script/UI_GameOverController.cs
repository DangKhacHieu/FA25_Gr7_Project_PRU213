using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_GameOverController : MonoBehaviour
{
    [Header("Panel Game Over (root)")]
    public GameObject panelGameOver;

    private bool shown = false;

    void Start()
    {
        if (panelGameOver != null)
            panelGameOver.SetActive(false);
    }

    public void ShowGameOver()
    {
        if (shown) return;
        shown = true;

        if (panelGameOver != null)
        {
            panelGameOver.SetActive(true);
            Time.timeScale = 0f;
            Debug.Log("💀 Hiển thị UI Game Over!");
        }
        else
        {
            Debug.LogWarning("⚠️ panelGameOver chưa được gán trong Inspector!");
        }
    }

    // ===================== Các nút =====================
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
