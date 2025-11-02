using UnityEngine;
using UnityEngine.UI; // Cần dùng thư viện UI

[RequireComponent(typeof(Button))] // Đảm bảo nó được gắn vào 1 nút

public class HeroSelectionButton_Lv3 : MonoBehaviour
{

    // Kéo PREFAB của tướng mà nút này đại diện vào đây
    [SerializeField] private GameObject heroPrefab;

    // Biến để lưu trữ PlacementManager
    private PlacementManager_Lv3 placementManager;
    private Button button;

    void Start()
    {
        // 1. Tìm PlacementManager trong Scene
        // (Cách này yêu cầu chỉ có 1 PlacementManager)
        placementManager = FindObjectOfType<PlacementManager_Lv3>();

        if (placementManager == null)
        {
            Debug.LogError("Không tìm thấy PlacementManager trong Scene!");
        }

        // 2. Lấy component Button và gán sự kiện OnClick
        button = GetComponent<Button>();

        // Khi nút này được BẤM, nó sẽ gọi hàm OnClickButton
        button.onClick.AddListener(OnClickButton);
    }

    void OnClickButton()
    {
        // Khi được bấm, nó sẽ gọi hàm SelectHeroToPlace trên PlacementManager
        // và "gửi" prefab tướng của nó vào
        if (heroPrefab != null && placementManager != null)
        {
            placementManager.SelectHeroToPlace(heroPrefab);
        }
        else
        {
            Debug.LogWarning("Nút " + gameObject.name + " chưa được gán Hero Prefab!");
        }
    }

}

