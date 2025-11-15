using UnityEngine;

public class UI_NextLevelController : MonoBehaviour
{
    [Header("Panel hiển thị khi thắng")]
    public GameObject missionCompletePanel;

    private void Start()
    {
        // Ẩn panel khi bắt đầu game
        if (missionCompletePanel != null)
            missionCompletePanel.SetActive(false);
    }

    /// <summary>
    /// Gọi hàm này để hiện UI thắng (ví dụ từ LevelCompleteManager)
    /// </summary>
    public void ShowMissionComplete()
    {
        if (missionCompletePanel != null)
        {
            missionCompletePanel.SetActive(true);
            Time.timeScale = 0f;
            Debug.Log("🏆 Hiện MissionCompletePanel!");
        }
        else
        {
            Debug.LogWarning("⚠️ MissionCompletePanel chưa được gán trong UI_NextLevelController!");
        }
    }

    /// <summary>
    /// Ẩn panel (nếu bạn muốn reset hoặc thử lại)
    /// </summary>
    public void HideMissionComplete()
    {
        if (missionCompletePanel != null)
        {
            missionCompletePanel.SetActive(false);
            Time.timeScale = 1f;
        }
    }
}
