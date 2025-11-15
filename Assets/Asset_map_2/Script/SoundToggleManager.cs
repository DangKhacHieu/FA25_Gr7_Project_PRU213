using UnityEngine;
using UnityEngine.UI;

public class SoundToggleManager : MonoBehaviour
{
    [Header("🎵 Cấu hình âm thanh")]
    public Button soundButton;       // Nút icon loa
    public Sprite soundOnSprite;     // Icon khi âm thanh bật
    public Sprite soundOffSprite;    // Icon khi âm thanh tắt

    private bool isMuted = false;

    void Start()
    {
        if (soundButton != null)
            soundButton.onClick.AddListener(ToggleSound);

        // 🔊 Khi mới vào game — mặc định BẬT âm thanh
        if (!PlayerPrefs.HasKey("GameMuted"))
            PlayerPrefs.SetInt("GameMuted", 0);

        isMuted = PlayerPrefs.GetInt("GameMuted") == 1;
        ApplySoundState();
    }

    void ToggleSound()
    {
        isMuted = !isMuted;
        PlayerPrefs.SetInt("GameMuted", isMuted ? 1 : 0);
        PlayerPrefs.Save();

        ApplySoundState();

        Debug.Log(isMuted ? "🔇 Âm thanh đã tắt toàn bộ" : "🔊 Âm thanh bật trở lại");
    }

    void ApplySoundState()
    {
        // ✅ Tắt / bật toàn bộ âm thanh (cả nhạc và hiệu ứng)
        AudioListener.volume = isMuted ? 0f : 1f;

        // ✅ Cập nhật icon nút loa
        if (soundButton != null)
            soundButton.image.sprite = isMuted ? soundOffSprite : soundOnSprite;
    }
}
