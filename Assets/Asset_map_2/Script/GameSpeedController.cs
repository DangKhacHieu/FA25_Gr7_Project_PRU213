using UnityEngine;
using UnityEngine.UI;

public class GameSpeedController : MonoBehaviour
{
    [Header("Buttons")]
    public Button speed05xButton;
    public Button speed1xButton;
    public Button speed2xButton;

    [Header("Màu sắc hiển thị")]
    public Color normalColor = Color.white;
    public Color selectedColor = Color.blue;

    private Button currentSelectedButton;

    void Start()
    {
        // Gán sự kiện bấm
        speed05xButton.onClick.AddListener(() => SetSpeed(0.5f, speed05xButton));
        speed1xButton.onClick.AddListener(() => SetSpeed(1f, speed1xButton));
        speed2xButton.onClick.AddListener(() => SetSpeed(2f, speed2xButton));

        // Mặc định tốc độ 1x
        SetSpeed(1f, speed1xButton);
    }

    void SetSpeed(float scale, Button selectedButton)
    {
        Time.timeScale = scale;
        Debug.Log("Tốc độ game: " + scale + "x");

        // Đổi màu nút
        if (currentSelectedButton != null)
            currentSelectedButton.image.color = normalColor;

        selectedButton.image.color = selectedColor;
        currentSelectedButton = selectedButton;
    }
}
