using TMPro;
using UnityEngine;
using System; // <-- THÊM VÀO để dùng 'Action'

public class CurrencyManager_Lv3 : MonoBehaviour
{
    // --- Singleton Pattern ---
    public static CurrencyManager_Lv3 Instance { get; private set; }

    // THÊM MỚI: Sự kiện để thông báo khi vàng thay đổi (hoặc khi cần reset)
    public static event Action<int> OnGoldChanged;

    // --- UI ---
    [Header("UI")]
    [SerializeField]
    private TextMeshProUGUI goldText;

    // --- DỮ LIỆU VÀNG ---
    private int _currentGold;
    public int CurrentGold
    {
        get => _currentGold;
        private set
        {
            _currentGold = value;
            UpdateGoldUI();
        }
    }

    private void Awake()
    {
        // --- Setup Singleton ---
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject);
        }
    }

    // --- THÊM MỚI: Thiết lập vàng ban đầu khi level được tải ---
    public void InitializeGold(int startingAmount)
    {
        CurrentGold = startingAmount;
    }

    // --- THÊM MỚI: Lắng nghe sự kiện từ Spawner ---
    private void OnEnable()
    {
        // Đăng ký lắng nghe sự kiện khi một wave được hoàn thành
        // (Chúng ta sẽ cần tạo sự kiện này trong Spawner)
        Spawner_Lv3.OnWaveCompleted += HandleWaveCompleted;
    }

    // --- THÊM MỚI: Ngừng lắng nghe khi tắt ---
    private void OnDisable()
    {
        Spawner_Lv3.OnWaveCompleted -= HandleWaveCompleted;
    }
    // --- HẾT THÊM MỚI ---

    private void Start()
    {
        // --- THAY ĐỔI: Thiết lập vàng ban đầu là 320 ---
        CurrentGold = LevelManager.Instance.CurrentLevel.startingResources;
    }

    /// <summary>
    /// Cộng vàng cho người chơi.
    /// </summary>
    public void AddGold(int amount)
    {
        if (amount <= 0) return;
        CurrentGold += amount;
    }

    /// <summary>
    /// Thử tiêu vàng (dùng để mua trụ, nâng cấp...).
    /// </summary>
    public bool SpendGold(int amount)
    {
        if (amount <= 0) return false;

        if (CurrentGold >= amount)
        {
            CurrentGold -= amount;
            return true;
        }
        else
        {
            return false;
        }
    }

    // --- HÀM MỚI: Xử lý khi nhận được tin báo wave đã xong ---
    private void HandleWaveCompleted(int waveNumber)
    {
        // Dựa vào số thứ tự của wave để thưởng vàng
        switch (waveNumber)
        {
            case 3:
                AddGold(60);
                Debug.Log($"Hoàn thành Wave 3! Thưởng 60 vàng.");
                break;
            case 5:
                AddGold(80);
                Debug.Log($"Hoàn thành Wave 5! Thưởng 80 vàng.");
                break;
            case 8:
                AddGold(100);
                Debug.Log($"Hoàn thành Wave 8! Thưởng 100 vàng.");
                break;
                // Bạn có thể thêm các mốc thưởng khác ở đây
        }
    }

    /// <summary>
    /// Cập nhật UI Text hiển thị số vàng.
    /// </summary>
    private void UpdateGoldUI()
    {
        if (goldText != null)
        {
            // --- THAY ĐỔI: Chỉ hiện số (để dùng với icon) ---
            goldText.text = $"{CurrentGold}";
        }
    }
}
