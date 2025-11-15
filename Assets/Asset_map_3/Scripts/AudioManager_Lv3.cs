using UnityEngine;
using UnityEngine.UI; // <-- THÊM DÒNG NÀY

public class AudioManager_Lv3 : MonoBehaviour
{// Biến này để lưu trạng thái đã tắt tiếng hay chưa
    private bool isMuted = false;

    // Key để lưu vào PlayerPrefs
    private const string MUTE_PREF_KEY = "IsMuted";

    [Header("Tùy chọn cho Nút Mute")]
    public Image muteButtonImage; // Kéo Image của cái nút Mute vào đây
    public Sprite mutedSprite;    // Hình khi đã tắt tiếng (cái loa gạch chéo)
    public Sprite unmutedSprite;  // Hình khi đang mở tiếng (cái loa bình thường)

    void Start()
    {
        // Khi game bắt đầu, kiểm tra xem trước đó người chơi có tắt tiếng không
        // PlayerPrefs.GetInt(key, defaultValue)
        // Nếu đã lưu "MUTE_PREF_KEY" = 1 -> isMuted = true
        // Nếu chưa lưu bao giờ, lấy giá trị mặc định là 0 -> isMuted = false
        isMuted = PlayerPrefs.GetInt(MUTE_PREF_KEY, 0) == 1;

        // Áp dụng trạng thái mute ngay lập tức
        ApplyMuteState();
    }

    // Hàm này sẽ được gọi bởi Nút (Button)
    public void ToggleMute()
    {
        // Đảo ngược trạng thái: true -> false, false -> true
        isMuted = !isMuted;

        // Áp dụng trạng thái mới
        ApplyMuteState();

        // Lưu trạng thái mới vào PlayerPrefs
        // Nếu isMuted là true, lưu số 1. Nếu là false, lưu số 0.
        PlayerPrefs.SetInt(MUTE_PREF_KEY, isMuted ? 1 : 0);
        PlayerPrefs.Save(); // Lưu lại ngay lập tức
    }

    private void ApplyMuteState()
    {
        if (isMuted)
        {
            // Tắt tiếng game
            AudioListener.volume = 0f;
            // Cập nhật hình ảnh nút
            if (muteButtonImage != null)
                muteButtonImage.sprite = mutedSprite;
        }
        else
        {
            // Mở tiếng game (âm lượng 100%)
            AudioListener.volume = 1f;
            // Cập nhật hình ảnh nút
            if (muteButtonImage != null)
                muteButtonImage.sprite = unmutedSprite;
        }
    }
}
