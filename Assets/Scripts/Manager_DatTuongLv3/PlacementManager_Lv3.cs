using UnityEngine;
using UnityEngine.EventSystems;

public class PlacementManager_Lv3 : MonoBehaviour
{
 
     // --- THÊM DÒNG NÀY VÀO ---
    // Biến này chỉ dùng để test, kéo Prefab vào từ Inspector
    [SerializeField] private GameObject heroToTestPrefab;
    // --- HẾT PHẦN THÊM ---

    // Biến này sẽ lưu tướng ĐANG ĐƯỢC CHỌN để đặt
    private GameObject heroToPlace;

    // (Giữ nguyên từ code cũ)
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        // heroToPlace = null; // Lúc bắt đầu game, chưa chọn tướng nào
        heroToPlace = heroToTestPrefab; // GÁN TƯỚNG TEST VÀO LÀM TƯỚNG ĐƯỢC CHỌN
    }

    void Update()
    {
        // Nếu game đang dừng (thắng/thua), không cho đặt
        if (Time.timeScale == 0f) return;

        // Kiểm tra nếu người chơi click chuột trái
        if (Input.GetMouseButtonDown(0))
        {
            // --- THÊM MỚI ---
            // Kiểm tra xem có click trúng UI không (trúng nút bấm, panel...)
            // Nếu click trúng UI, thì không phải là click để đặt tướng -> Dừng lại
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            // Nếu không click trúng UI, thì mới xử lý đặt tướng
            HandlePlacement();
        }
    }

    // --- HÀM CÔNG KHAI (PUBLIC) MỚI ---
    // Các nút UI sẽ gọi hàm này để "chọn" tướng
    public void SelectHeroToPlace(GameObject heroPrefab)
    {
        heroToPlace = heroPrefab;
        Debug.Log("Đã chọn tướng: " + heroPrefab.name);
    }

    // --- HÀM XỬ LÝ ĐẶT (ĐÃ SỬA ĐỔI) ---
    void HandlePlacement()
    {
        // --- THÊM MỚI ---
        // Nếu chưa chọn tướng nào (heroToPlace là null) thì không làm gì cả
        if (heroToPlace == null)
        {
            Debug.Log("Vui lòng chọn một tướng từ UI trước!");
            return;
        }

        // (Giữ nguyên code kiểm tra vị trí từ trước)
        Vector2 mousePosition = Input.mousePosition;
        Vector2 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
        Collider2D[] colliders = Physics2D.OverlapPointAll(worldPosition);

        bool canPlace = false;
        bool onPath = false;

        foreach (Collider2D col in colliders)
        {
            if (col.CompareTag("Path"))
            {
                onPath = true;
                break;
            }
            if (col.CompareTag("Ground"))
            {
                canPlace = true;
            }
        }

        // Logic cuối cùng:
        if (canPlace && !onPath)
        {
            // --- SỬA ĐỔI ---
            // Đặt con tướng đã được chọn (heroToPlace)
            Instantiate(heroToPlace, worldPosition, Quaternion.identity);
            Debug.Log("Đã đặt tướng!");

            // --- TÙY CHỌN ---
            // Nếu bạn muốn sau khi đặt 1 tướng, nó tự bỏ chọn
            // (bắt người chơi phải click lại nút UI)
            // thì hãy bỏ dấu // ở dòng dưới:
            // heroToPlace = null;
        }
        else
        {
            Debug.Log("Không thể đặt tướng tại đây!");
        }
    }
}
