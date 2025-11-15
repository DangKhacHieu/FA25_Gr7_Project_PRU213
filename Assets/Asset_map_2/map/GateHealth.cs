using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GateHealth : MonoBehaviour
{
    [Header("Cấu hình mạng cổng thành")]
    public int maxHealth = 5;
    public int currentHealth;

    [Header("Tham chiếu UI")]
    public TextMeshProUGUI liveText;   // 👈 Gắn LiveText vào đây
    public GameObject gameOverUI;      // UI thua game (nếu có)

    [Header("Hiệu ứng khi bị tấn công (tuỳ chọn)")]
    public GateShake gateShake;
    public AudioSource hitSound;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateLiveText();
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log($"🏰 Gate bị tấn công! -{amount} HP (còn {currentHealth})");

        UpdateLiveText();

        if (gateShake != null) gateShake.ShakeGate();
        if (hitSound != null) hitSound.Play();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateLiveText()
    {
        if (liveText != null)
        {
            liveText.text = "" + currentHealth;
        }
    }

    private void Die()
    {
        Debug.Log("💥 Cổng thành đã bị phá! Game Over!");
        ShowGameOver();
    }

    private void ShowGameOver()
    {
        if (gameOverUI != null)
        {
            UI_GameOverController controller = gameOverUI.GetComponent<UI_GameOverController>();
            if (controller != null)
                controller.ShowGameOver();
            else
                gameOverUI.SetActive(true);
        }
        else
        {
            Debug.LogWarning("⚠️ GameOverUI chưa được gán trong Inspector!");
        }

        Time.timeScale = 0f; // Dừng game
    }
        
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public bool IsAlive()
    {
        return currentHealth > 0;
    }
}
