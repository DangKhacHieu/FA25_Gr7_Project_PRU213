using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiController_Lv3 : MonoBehaviour
{
    [SerializeField] private TMP_Text waveText;
    [SerializeField] private TMP_Text LiveText;

    [SerializeField] private GameObject heroPanel;
    [SerializeField] private GameObject heroCardPrefab;
    [SerializeField] private Transform cardsContainer;

    [SerializeField] private HeroData_Lv3[] heros;
    private List<GameObject> activeCards = new List<GameObject>();

    private Platform_Lv3 _currenrPlatform;

    [SerializeField] private Button speed1Button;
    [SerializeField] private Button speed2Button;
    [SerializeField] private Button speed3Button;

    [SerializeField] private Color normalButtonColor = Color.white;
    [SerializeField] private Color selectedButtonColor = Color.blue;
    [SerializeField] private Color normalTextColor = Color.black;
    [SerializeField] private Color selectedTextColor = Color.white;

    [SerializeField] private GameObject pausePanel;
    private bool _isGamePaused = false;

    [SerializeField] private GameObject missionCompletePanel;
    [SerializeField] private GameObject GameOverPanel;

    // --- THÊM VÀO ĐÂY ---
    [Header("Audio")]
    // Kéo component AudioSource (ở Bước 1) vào đây
    [SerializeField] private AudioSource uiAudioSource;
    // Kéo file âm thanh "click" vào đây
    [SerializeField] private AudioClip genericClickSound;
    // Kéo file âm thanh "không đủ tiền" / "lỗi" vào đây
    [SerializeField] private AudioClip errorSound;
    // --- THÊM 2 DÒNG NÀY ---
    [SerializeField] private AudioClip winSound;    // Kéo âm thanh thắng vào đây
    [SerializeField] private AudioClip loseSound;   // Kéo âm thanh thua vào đây
    // --- HẾT THÊM MỚI ---
    // --- HẾT THÊM MỚI ---

    private void OnEnable()
    {
        Spawner_Lv3.OnWaveChange += UpdateWaveText;
        GameManager_Lv3.OnLivesChanged += UpdateLiveText;
        Platform_Lv3.OnPlatformClicked += HandlePlatformClicked;
        HeroCard_Lv3.OnHeroSelected += HandleHeroSelected;

        // --- THÊM DÒNG NÀY ---
        Spawner_Lv3.OnAllWavesCompleted += ShowWinPanel;
    }

    private void OnDisable()
    {
        Spawner_Lv3.OnWaveChange -= UpdateWaveText;
        GameManager_Lv3.OnLivesChanged -= UpdateLiveText;
        Platform_Lv3.OnPlatformClicked -= HandlePlatformClicked;
        HeroCard_Lv3.OnHeroSelected -= HandleHeroSelected;
        // --- THÊM DÒNG NÀY ---
        Spawner_Lv3.OnAllWavesCompleted -= ShowWinPanel;
    }

    private void Start()
    {
        // --- SỬA LẠI CÁC LISTENER NÀY ---
        // Thêm hàm PlayClickSound() vào
        speed1Button.onClick.AddListener(() => { SetGameSpeed(0.2f); PlayClickSound(); });
        speed2Button.onClick.AddListener(() => { SetGameSpeed(1f); PlayClickSound(); });
        speed3Button.onClick.AddListener(() => { SetGameSpeed(2f); PlayClickSound(); });
        // ---------------------------------

        HighlightSelectedSpeedButton(GameManager_Lv3.Instance.GameSpeed);

        // --- THÊM VÀO ĐÂY (Để an toàn) ---
        // Tự lấy AudioSource nếu bạn quên kéo vào
        if (uiAudioSource == null)
        {
            uiAudioSource = GetComponent<AudioSource>();
            if (uiAudioSource == null) // Nếu vẫn không có, tự tạo 1 cái
            {
                uiAudioSource = gameObject.AddComponent<AudioSource>();
                uiAudioSource.playOnAwake = false;
            }
        }
        // --- HẾT THÊM MỚI ---
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }
    }


    private void UpdateWaveText(int currentWave)
    {
        waveText.text = $"wave: {currentWave++}";
    }

    private void UpdateLiveText(int currentLives)
    {
        LiveText.text = $"{currentLives}";
        // --- THÊM LOGIC KIỂM TRA THUA VÀO ĐÂY ---
        // Chỉ chạy 1 lần khi thua (tránh việc gọi SetActive liên tục)
        if (currentLives <= 0 && !GameOverPanel.activeSelf)
        {
            // 1. Kích hoạt Panel Game Over
            GameOverPanel.SetActive(true);

            // 2. Dừng game
            GameManager_Lv3.Instance.SetTimeScale(0f);

            // 3. Phát âm thanh THUA
            if (loseSound != null && uiAudioSource != null)
            {
                uiAudioSource.PlayOneShot(loseSound);
            }
        }
        // --- HẾT THÊM MỚI ---
    }

    private void HandlePlatformClicked(Platform_Lv3 platform_Lv3)
    {
        _currenrPlatform = platform_Lv3;
        ShowHeroPanel();
    }

    private void ShowHeroPanel()
    {
        heroPanel.SetActive(true);
        Platform_Lv3.heroPanelOpen = true;
        GameManager_Lv3.Instance.SetTimeScale(0f);
        PopulateHeroCards();
    }

    public void HideHeroPanel()
    {
        heroPanel.SetActive(false);
        Platform_Lv3.heroPanelOpen = false;
        GameManager_Lv3.Instance.SetTimeScale(1f);
    }

    private void PopulateHeroCards()
    {
        foreach(var card in activeCards)
        {
            Destroy(card);
        }activeCards.Clear();

        foreach(var data in heros)
        {
            GameObject cardGameObject = Instantiate(heroCardPrefab,
                cardsContainer);
            HeroCard_Lv3 card = cardGameObject.GetComponent<HeroCard_Lv3>();
            card.Initialize(data);
            activeCards.Add(cardGameObject);
        }
    }

    private void HandleHeroSelected(HeroData_Lv3 heroData)
    {

        // _currenrPlatform.PlaceHero(heroData);
        //HideHeroPanel();
        // 1. Kiểm tra xem CurrencyManager có tồn tại không
        if (CurrencyManager_Lv3.Instance == null)
        {
            Debug.LogError("CurrencyManager.Instance không tồn tại!");
            HideHeroPanel(); // Vẫn ẩn panel đi
            return;
        }

        // 2. Thử tiêu tiền
        bool success = CurrencyManager_Lv3.Instance.SpendGold(heroData.cost);

        // 3. Nếu tiêu tiền thành công
        if (success)
        {
            // 4. Mới đặt lính
            _currenrPlatform.PlaceHero(heroData);
        }
        else
        {
            // Bạn có thể thêm 1 âm thanh "báo lỗi" ở đây
            // --- THÊM VÀO ĐÂY ---
            // Phát tiếng báo lỗi
            if (errorSound != null && uiAudioSource != null)
            {
                uiAudioSource.PlayOneShot(errorSound);
            }
            // --- HẾT THÊM MỚI ---
            // 5. Nếu không đủ tiền, thông báo (hoặc làm gì đó khác)
            Debug.Log($"Không đủ tiền! Cần {heroData.cost} vàng.");
            
        }

        // 6. Dù thành công hay thất bại, cũng ẩn panel chọn lính
        HideHeroPanel();
    }
    // --- KẾT THÚC THAY ĐỔI ---
   /* private IEnumerator ShowNoResourcesMessage()
    {
        noResourcesText.SetActive(true);
        yield return new WaitForSecondsRealtime(3f);
        noResourcesText.SetActive(false);
    }*/

    private void SetGameSpeed(float timeScale)
    {
        HighlightSelectedSpeedButton(timeScale);
        GameManager_Lv3.Instance.SetGameSpeed(timeScale);
    }

    private void UpdateButtonVisual(Button button, bool isSelected)
    {
        button.image.color = isSelected ? selectedButtonColor : normalButtonColor;

        TMP_Text text = button.GetComponentInChildren<TMP_Text>();
        if (text != null)
        {
            text.color = isSelected ? selectedTextColor : normalTextColor;
        }
    }

    private void HighlightSelectedSpeedButton(float selectedSpeed)
    {
        UpdateButtonVisual(speed1Button, selectedSpeed == 0.2f);
        UpdateButtonVisual(speed2Button, selectedSpeed == 1f);
        UpdateButtonVisual(speed3Button, selectedSpeed == 2f);
    }

    public void TogglePause()
    {
        if (heroPanel.activeSelf)
            return;
        PlayClickSound(); // Thêm vào đây

        if (_isGamePaused)
        {
            pausePanel.SetActive(false);
            _isGamePaused = false;
            GameManager_Lv3.Instance.SetTimeScale(GameManager_Lv3.Instance.GameSpeed);
        }
        else
        {
            pausePanel.SetActive(true);
            _isGamePaused = true;
            GameManager_Lv3.Instance.SetTimeScale(0f);
        }
    }

    public void RestartLevel()
    {
        GameManager_Lv3.Instance.SetTimeScale(1f); // Đảm bảo time scale về 1
        PlayClickSound(); // Thêm vào đây

        // Load lại CurrentLevel của LevelManager
        if (LevelManager.Instance != null && LevelManager.Instance.CurrentLevel != null)
        {
            LevelManager.Instance.LoadLevel(LevelManager.Instance.CurrentLevel);
        }
        else
        {
            // Trường hợp LevelManager chưa kịp khởi tạo hoặc bị mất
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.buildIndex);
        }
    }

    public void QuitGame()
    {
        PlayClickSound(); // Thêm vào đây
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void GoToMainMenu()
    {
        PlayClickSound(); // Thêm vào đây
        GameManager_Lv3.Instance.SetTimeScale(1f);
        SceneManager.LoadScene("MainMenu");
    }
    public void LoadNextLevel()
    {
        PlayClickSound(); // Thêm vào đây
        var levelManager = LevelManager.Instance;
        int currentIndex = Array.IndexOf(levelManager.allLevels, levelManager.CurrentLevel);
        int nextIndex = currentIndex + 1;
        if (nextIndex < levelManager.allLevels.Length)
        {
            missionCompletePanel.SetActive(false);
            levelManager.LoadLevel(levelManager.allLevels[nextIndex]);
        }
    }

    public void EnterEndlessMode()
    {
        PlayClickSound(); // Thêm vào đây
        // 1. Ẩn panel Mission Complete
        missionCompletePanel.SetActive(false);

        // 2. Khôi phục tốc độ thời gian game (vì ShowMissionComplete đã set timeScale = 0)
        GameManager_Lv3.Instance.SetTimeScale(GameManager_Lv3.Instance.GameSpeed);

        // 3. Kích hoạt Endless Mode trong Spawner
        // Đảm bảo Spawner.Instance đã được thiết lập.
        if (Spawner_Lv3.Instance != null)
        {
            Spawner_Lv3.Instance.EnableEndlessMode();
        }
        else
        {
            Debug.LogError("Spawner Instance is missing! Cannot enter Endless Mode.");
        }
       
    }

    // --- THÊM CÁC HÀM NÀY VÀO CUỐI SCRIPT ---

    // Hàm public để các nút khác (trong Inspector) có thể gọi
    public void PlayClickSound()
    {
        if (genericClickSound != null && uiAudioSource != null)
        {
            // PlayOneShot cho phép phát nhiều tiếng đè lên nhau,
            // không cắt ngang âm thanh đang phát
            uiAudioSource.PlayOneShot(genericClickSound);
        }
    }

    // --- THÊM NGUYÊN HÀM MỚI NÀY VÀO ---
    private void ShowWinPanel()
    {
        // 1. Kích hoạt Panel Thắng
        missionCompletePanel.SetActive(true);

        // 2. Dừng game
        GameManager_Lv3.Instance.SetTimeScale(0f);

        // 3. Phát âm thanh THẮNG
        if (winSound != null && uiAudioSource != null)
        {
            uiAudioSource.PlayOneShot(winSound);
        }
    }
}


