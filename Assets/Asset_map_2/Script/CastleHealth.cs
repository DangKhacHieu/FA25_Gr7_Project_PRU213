using UnityEngine;
using UnityEngine.UI;

public class CastleHealth : MonoBehaviour
{
    [Header("HP / Lives")]
    public int lives = 5;          // thua sau 5 con
    public Slider healthBar;       // kéo Slider GateHP vào

    [Header("Options")]
    public bool destroyEnemyOnHit = true;

    void Start()
    {
        lives = LevelManager.Instance.CurrentLevel.startingLives;
        if (healthBar != null)
        {
            healthBar.minValue = 0;
            healthBar.maxValue = lives;
            healthBar.value = lives;
        }
        Time.timeScale = 1f;
    }

    public void LoseLife(int amount = 1, GameObject enemy = null)
    {
        lives = Mathf.Max(0, lives - amount);
        if (healthBar != null) healthBar.value = lives;

        if (enemy && destroyEnemyOnHit) Destroy(enemy);

        if (lives <= 0) GameOver();
    }

    void GameOver()
    {
        Time.timeScale = 0f;
        Debug.Log("GAME OVER");
        // TODO: hiện panel/thay scene nếu muốn
    }
    public void TakeDamage(int amount)
    {
        lives -= amount;

        if (healthBar != null)
        {
            healthBar.value = lives;
        }

        if (lives <= 0)
        {
            Debug.Log("🏰 Castle Destroyed!");
            // Bạn có thể thêm game over hoặc dừng game ở đây
        }
    }

}
