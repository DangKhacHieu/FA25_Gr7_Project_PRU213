using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEngine.UI; // Cần thiết nếu bạn muốn đổi màu nút
using TMPro; // Cần thiết nếu bạn muốn đổi chữ

public class MainMenuController : MonoBehaviour
{
    // Dùng 'static' để trạng thái mute được "nhớ" khi qua màn chơi khác
    private static bool _isMuted = false;

    // ----- CÁC BIẾN MỚI -----
    // Bạn có thể dùng 1 trong 2 cách sau để cho người dùng biết nút đã được bấm:

   

    // Cách 2: Gán Button vào đây để đổi màu (giống như bạn làm ở Map 1)
    [SerializeField] private Button muteButton;
    [SerializeField] private Color selectedButtonColor = Color.blue;
    [SerializeField] private Color normalButtonColor = Color.white;
    // -------------------------

    void Start()
    {
        // Khi Main Menu vừa tải,
        // kiểm tra trạng thái mute đã lưu và cập nhật lại âm thanh/giao diện
        UpdateMuteVisuals();
    }

    public void StartNewGame()
    {
        LevelManager.Instance.LoadLevel(LevelManager.Instance.allLevels[0]);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    // ----- HÀM MỚI ĐỂ GỌI TỪ NÚT BẤM -----
    public void ToggleMute()
    {
        // 1. Đảo ngược trạng thái
        _isMuted = !_isMuted;

        // 2. Cập nhật âm thanh và giao diện
        UpdateMuteVisuals();
    }

    // ----- HÀM MỚI ĐỂ CẬP NHẬT GIAO DIỆN -----
    private void UpdateMuteVisuals()
    {
        if (_isMuted)
        {
            // TẮT tiếng
            AudioListener.volume = 0f;

            
            // Cập nhật màu nút (nếu bạn dùng cách 2)
            if (muteButton != null)
            {
                muteButton.image.color = selectedButtonColor;
            }
        }
        else
        {
            // BẬT tiếng
            AudioListener.volume = 1f;

           
            // Cập nhật màu nút (nếu bạn dùng cách 2)
            if (muteButton != null)
            {
                muteButton.image.color = normalButtonColor;
            }
        }
    }
}
