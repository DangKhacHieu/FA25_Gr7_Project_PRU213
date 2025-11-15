using System;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Lumin;

public class Platform_Lv3 : MonoBehaviour
{
    /*public static event Action<Platform_Lv3> OnPlatformClicked;
    [SerializeField] private LayerMask platformLayerMask;

    // --- CÁC BIẾN MỚI CHO VIỆC "BÁN" TƯỚNG ---

    // Biến này lưu tham chiếu đến con tướng đã đặt
    private GameObject _placedHero;

    // Biến này dùng để kiểm tra double-click
    private float _lastClickTime = 0f;

    // Ngưỡng thời gian cho double-click (tính bằng giây), 
    // bạn có thể chỉnh trong Inspector
    [SerializeField] private float doubleClickThreshold = 0.3f;

    // --- KẾT THÚC BIẾN MỚI ---

    // --- THÊM BIẾN NÀY ---
    private bool isOccupied = false;
    // --- KẾT THÚC THÊM ---
    public static bool heroPanelOpen { get; set; } = false;
    private void Update()
    {
        if (heroPanelOpen)
        {
            return;
        }
        // --- THÊM ĐOẠN KIỂM TRA NÀY ---
        // Nếu ô này đã có lính, thì không cho bấm nữa
        if (isOccupied)
        {
            return;
        }
        // --- KẾT THÚC THÊM ---

        if(heroPanelOpen || Time.timeScale == 0f)
        {
            return; 
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 worlPoint = Camera.main.ScreenToWorldPoint(Mouse.current.
                position.ReadValue());
            RaycastHit2D raycastHit = Physics2D.Raycast(worlPoint, Vector2.zero,
                Mathf.Infinity, platformLayerMask);
            
            if (raycastHit.collider != null)
            {
                Platform_Lv3 platform_Lv3 = raycastHit.collider.GetComponent<Platform_Lv3>();
                if(platform_Lv3 != null)
                {
                    OnPlatformClicked?.Invoke(platform_Lv3);
                }
            }
        }
    }

    public void PlaceHero(HeroData_Lv3 data)
    {
        Instantiate(data.prefab, transform.position, Quaternion.identity, transform);

        // --- THÊM DÒNG NÀY ĐỂ "ĐÁNH DẤU" Ô NÀY ---
        isOccupied = true;
        // --- KẾT THÚC THÊM ---
    }*/

    public static event Action<Platform_Lv3> OnPlatformClicked;
    [SerializeField] private LayerMask platformLayerMask;

    // --- CÁC BIẾN MỚI CHO VIỆC "BÁN" TƯỚNG ---

    // Biến này lưu tham chiếu đến con tướng đã đặt
    private GameObject _placedHero;

    // Biến này dùng để kiểm tra double-click
    private float _lastClickTime = 0f;

    // Ngưỡng thời gian cho double-click (tính bằng giây), 
    // bạn có thể chỉnh trong Inspector
    [SerializeField] private float doubleClickThreshold = 0.3f;

    // --- KẾT THÚC BIẾN MỚI ---

    private bool isOccupied = false;
    public static bool heroPanelOpen { get; set; } = false;

    // --- HÀM UPDATE ĐÃ ĐƯỢC THAY ĐỔI HOÀN TOÀN ---
    private void Update()
    {
        // 1. Nếu panel hero đang mở hoặc game đang pause, không làm gì cả
        if (heroPanelOpen || Time.timeScale == 0f)
        {
            return;
        }

        // 2. Chỉ xử lý khi nhấn chuột trái
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 worlPoint = Camera.main.ScreenToWorldPoint(Mouse.current.
                position.ReadValue());
            RaycastHit2D raycastHit = Physics2D.Raycast(worlPoint, Vector2.zero,
                Mathf.Infinity, platformLayerMask);

            // 3. Kiểm tra xem có click trúng collider VÀ collider đó CÓ PHẢI LÀ CỦA platform NÀY không
            if (raycastHit.collider != null && raycastHit.collider.gameObject == this.gameObject)
            {
                // 4. Logic rẽ nhánh: Xử lý MUA hay BÁN
                if (isOccupied)
                {
                    // --- XỬ LÝ BÁN TƯỚNG (DOUBLE CLICK) ---

                    // Kiểm tra xem thời gian từ cú click trước có đủ gần không
                    if (Time.time - _lastClickTime < doubleClickThreshold)
                    {
                        // Đây là một cú double-click!
                        SellHero();
                    }

                    // Cập nhật lại thời gian của cú click cuối cùng (dù là click 1 hay 2)
                    _lastClickTime = Time.time;
                }
                else
                {
                    // --- XỬ LÝ MUA TƯỚNG (SINGLE CLICK) ---

                    // Ô này đang trống, gửi sự kiện để mở panel
                    OnPlatformClicked?.Invoke(this);

                    // Reset thời gian click để tránh nhầm lẫn
                    _lastClickTime = 0f;
                }
            }
        }
    }

    // --- HÀM PLACEHERO ĐÃ ĐƯỢC SỬA ĐỔI ---
    public void PlaceHero(HeroData_Lv3 data)
    {
        // Tạo ra con tướng và LƯU LẠI tham chiếu của nó vào biến _placedHero
        _placedHero = Instantiate(data.prefab, transform.position, Quaternion.identity, transform);
        isOccupied = true;
    }

    // --- HÀM MỚI ĐỂ BÁN TƯỚNG ---
    private void SellHero()
    {
        Debug.Log("Hero sold!"); // In ra console để kiểm tra

        // Hủy đối tượng con tướng
        Destroy(_placedHero);

        // Reset lại trạng thái của platform
        _placedHero = null;
        isOccupied = false;

        // Reset thời gian click để ngăn cú click thứ 3, 4
        _lastClickTime = 0f;

        // Theo yêu cầu của bạn, chúng ta KHÔNG trả lại vàng ở đây
    }
}
