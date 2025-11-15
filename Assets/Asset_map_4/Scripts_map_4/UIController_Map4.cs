using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System;

// 1. Yêu cầu GameObject này phải có component AudioSource
[RequireComponent(typeof(AudioSource))]
public class UIController_map_4 : MonoBehaviour
{
    public static UIController_map_4 Instance { get; private set; }

    [Header("HUD")]
    [SerializeField] private TMP_Text waveText;
    [SerializeField] private TMP_Text livesText;
    [SerializeField] private TMP_Text resourcesText;
    [SerializeField] private TMP_Text warningText;

    [Header("Unit Panel (Towers/Heroes)")]
    [SerializeField] private GameObject HeroPanel;
    [SerializeField] private Transform cardsContainer;

    [Header("Hero Source")]
    [SerializeField] private GameObject heroCardPrefab;
    [SerializeField] private HeroData[] heroes;

    [Header("Tab Buttons (optional)")]
    [SerializeField] private Button tabTowersButton;
    [SerializeField] private Button tabHeroesButton;

    private readonly List<GameObject> activeCards = new();
    private Platform_map_4 _currentPlatform;

    [Header("Sound")]
    [SerializeField] private Image soundButtonImage;
    [SerializeField] private Color soundOnColor = Color.green;
    [SerializeField] private Color soundOffColor = Color.white;
    private bool _isMuted = false;

    // 2. Biến để giữ AudioSource (sẽ được gán trong Awake)
    private AudioSource audioSource;

    [Header("Speed Buttons")]
    [SerializeField] private Button speed1Button;
    [SerializeField] private Button speed2Button;
    [SerializeField] private Button speed3Button;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button nextLevelButton;

    [SerializeField] private Color normalButtonColor = Color.white;
    [SerializeField] private Color selectedButtonColor = Color.blue;
    [SerializeField] private Color normalTextColor = Color.black;
    [SerializeField] private Color selectedTextColor = Color.white;

    [Header("Panels")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text objectiveText;
    [SerializeField] private GameObject missionCompletePanel;

    private bool _isGamePaused = false;

    private enum UnitTab { Towers, Heroes }
    private UnitTab _currentTab = UnitTab.Towers;

    [Header("UI Fallbacks")]
    [SerializeField] private Vector2 fallbackCardSize = new Vector2(220, 120);

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); }
        else { Instance = this; }

        // 3. Lấy component AudioSource và cài đặt
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.loop = true; // Nhạc nền nên lặp lại
        audioSource.playOnAwake = false; // Chúng ta sẽ bật thủ công
    }

    private void OnEnable()
    {
        Spawner_map_4.OnWaveChanged += UpdateWaveText;
        GameManager_map_4.OnLivesChanged += UpdateLivesText;
        GameManager_map_4.OnResourcesChanged += UpdateResourcesText;
        Platform_map_4.OnPlatformClicked += HandlePlatformClicked;
        HeroCard_map_4.OnHeroSelected += HandleHeroSelected;

        SceneManager.sceneLoaded += OnSceneLoaded;
        Spawner_map_4.OnMissionComplete += ShowMissionComplete;

        if (tabTowersButton) tabTowersButton.onClick.AddListener(ShowTowersTab);
        if (tabHeroesButton) tabHeroesButton.onClick.AddListener(ShowHeroesTab);
    }

    private void OnDisable()
    {
        Spawner_map_4.OnWaveChanged -= UpdateWaveText;
        GameManager_map_4.OnLivesChanged -= UpdateLivesText;
        GameManager_map_4.OnResourcesChanged -= UpdateResourcesText;
        Platform_map_4.OnPlatformClicked -= HandlePlatformClicked;
        HeroCard_map_4.OnHeroSelected -= HandleHeroSelected;

        SceneManager.sceneLoaded -= OnSceneLoaded;
        Spawner_map_4.OnMissionComplete -= ShowMissionComplete;

        if (tabTowersButton) tabTowersButton.onClick.RemoveAllListeners();
        if (tabHeroesButton) tabHeroesButton.onClick.RemoveAllListeners();
    }

    private void Start()
    {
        // 4. Chỉ xử lý nút Mute trong Start
        _isMuted = PlayerPrefs.GetInt("IsMuted", 0) == 1;
        AudioListener.volume = _isMuted ? 0f : 1f;

        if (soundButtonImage != null)
        {
            soundButtonImage.color = _isMuted ? soundOffColor : soundOnColor;
        }

        speed1Button.onClick.AddListener(() => SetGameSpeed(0.2f));
        speed2Button.onClick.AddListener(() => SetGameSpeed(1f));
        speed3Button.onClick.AddListener(() => SetGameSpeed(2f));
        HighlightSelectedSpeedButton(GameManager_map_4.Instance.GameSpeed);
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame) TogglePause();
    }

    // ===== HUD =====
    private void UpdateWaveText(int currentWave) => waveText.text = $"Wave: {currentWave + 1}";
    private void UpdateLivesText(int currentLives)
    {
        livesText.text = $"{currentLives}";
        if (currentLives <= 0) ShowGameOver();
    }
    private void UpdateResourcesText(int currentResources) => resourcesText.text = $"{currentResources}";

    // ===== Placement flow =====
    private void HandlePlatformClicked(Platform_map_4 platform)
    {
        _currentPlatform = platform;
        ShowUnitPanel(_currentTab);
    }

    private void ShowUnitPanel(UnitTab tab)
    {
        _currentTab = tab;
        HeroPanel.SetActive(true);
        Platform_map_4.towerPanelOpen = true;
        GameManager_map_4.Instance.SetTimeScale(0f);

        if (tabTowersButton) UpdateButtonVisual(tabTowersButton, tab == UnitTab.Towers);
        if (tabHeroesButton) UpdateButtonVisual(tabHeroesButton, tab == UnitTab.Heroes);

        PopulateCards(tab);
    }

    public void ShowTowersTab() => ShowUnitPanel(UnitTab.Towers);
    public void ShowHeroesTab() => ShowUnitPanel(UnitTab.Heroes);

    public void HideTowerPanel()
    {
        HeroPanel.SetActive(false);
        Platform_map_4.towerPanelOpen = false;
        GameManager_map_4.Instance.SetTimeScale(GameManager_map_4.Instance.GameSpeed);
    }

    private void PopulateCards(UnitTab tab)
    {
        foreach (var go in activeCards) Destroy(go);
        activeCards.Clear();

        if (cardsContainer == null) return;

        bool hasLayout = cardsContainer.GetComponent<HorizontalOrVerticalLayoutGroup>() != null
                         || cardsContainer.GetComponent<GridLayoutGroup>() != null;

        if (heroes != null)
        {
            foreach (var data in heroes)
            {
                if (!data) continue;
                var cardGO = Instantiate(heroCardPrefab, cardsContainer, false);
                var card = cardGO.GetComponent<HeroCard_map_4>() ?? cardGO.GetComponentInChildren<HeroCard_map_4>(true);
                if (card) card.Initialize(data);
                EnsureUICardSetup(cardGO, hasLayout);
                activeCards.Add(cardGO);
            }
        }

        var rt = cardsContainer as RectTransform;
        if (rt != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
            Canvas.ForceUpdateCanvases();
        }
    }

    private void EnsureUICardSetup(GameObject cardGO, bool containerHasLayout)
    {
        var rt = cardGO.transform as RectTransform;
        if (rt == null) return;

        cardGO.SetActive(true);
        rt.localScale = Vector3.one;

        if (!containerHasLayout)
        {
            rt.anchorMin = new Vector2(0.5f, 1f);
            rt.anchorMax = new Vector2(0.5f, 1f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition3D = Vector3.zero;
            rt.sizeDelta = fallbackCardSize;
        }
    }

    private void HandleHeroSelected(HeroData heroData)
    {
        if (heroData == null || !CanPlace()) return;

        if (GameManager_map_4.Instance.Resources >= heroData.cost)
        {
            GameManager_map_4.Instance.SpendResources(heroData.cost);
            _currentPlatform.PlaceHero(heroData);
            HideTowerPanel();
        }
        else
        {
            StartCoroutine(ShowWarningMessage("Not enough resources!"));
        }
    }

    private bool CanPlace()
    {
        if (_currentPlatform == null) return false;
        if (_currentPlatform.transform.childCount > 0)
        {
            HideTowerPanel();
            StartCoroutine(ShowWarningMessage("This platform already has a unit!"));
            return false;
        }
        return true;
    }

    private IEnumerator ShowWarningMessage(string message)
    {
        warningText.text = message;
        warningText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(3f);
        warningText.gameObject.SetActive(false);
    }

    // ===== Speed / Pause / Scene =====
    private void SetGameSpeed(float timeScale)
    {
        HighlightSelectedSpeedButton(timeScale);
        GameManager_map_4.Instance.SetGameSpeed(timeScale);
    }

    private void UpdateButtonVisual(Button button, bool isSelected)
    {
        button.image.color = isSelected ? selectedButtonColor : normalButtonColor;
        var text = button.GetComponentInChildren<TMP_Text>();
        if (text) text.color = isSelected ? selectedTextColor : normalTextColor;
    }

    private void HighlightSelectedSpeedButton(float selectedSpeed)
    {
        UpdateButtonVisual(speed1Button, selectedSpeed == 0.2f);
        UpdateButtonVisual(speed2Button, selectedSpeed == 1f);
        UpdateButtonVisual(speed3Button, selectedSpeed == 2f);
    }

    public void TogglePause()
    {
        if (HeroPanel.activeSelf) return;

        if (_isGamePaused)
        {
            pausePanel.SetActive(false);
            _isGamePaused = false;
            GameManager_map_4.Instance.SetTimeScale(GameManager_map_4.Instance.GameSpeed);
        }
        else
        {
            pausePanel.SetActive(true);
            _isGamePaused = true;
            GameManager_map_4.Instance.SetTimeScale(0f);
        }
    }

    // ===== Scene / Result =====
    public void RestartLevel() => LevelManager.Instance.LoadLevel(LevelManager.Instance.CurrentLevel);
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    public void GoToMainMenu() { GameManager_map_4.Instance.SetTimeScale(1f); SceneManager.LoadScene("MainMenu"); }

    private void ShowGameOver() { GameManager_map_4.Instance.SetTimeScale(0f); gameOverPanel.SetActive(true); }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        var mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        GetComponent<Canvas>().worldCamera = mainCamera;

        HidePanels();

        // 5. CHỈNH SỬA LOGIC KHI TẢI SCENE
        if (scene.name == "MainMenu")
        {
            HideUI();
            Debug.Log("nhac ngung chay");
            audioSource.Stop(); // Tắt nhạc ở Main Menu
        }
        else
        {
            ShowUI();
            StartCoroutine(ShowObjective());
            Debug.Log("nhac chay");
            // Lấy nhạc từ LevelManager và phát
            PlayBackgroundMusic();
        }
    }

    // 6. HÀM MỚI ĐỂ PHÁT NHẠC
    private void PlayBackgroundMusic()
    {
        // Giả định rằng LevelManager đang giữ LevelData hiện tại
        if (LevelManager.Instance == null) return;

        LevelData currentLevelData = LevelManager.Instance.CurrentLevel;

        if (currentLevelData != null && currentLevelData.backgroundMusic != null)
        {
            // Nếu nhạc mới khác nhạc cũ, hoặc đang không bật
            if (audioSource.clip != currentLevelData.backgroundMusic || !audioSource.isPlaying)
            {
                audioSource.clip = currentLevelData.backgroundMusic;
                audioSource.Play();
            }
        }
        else
        {
            // Không có nhạc cho level này
            audioSource.Stop();
        }
    }

    private IEnumerator ShowObjective()
    {
        objectiveText.text = $"Survive {LevelManager.Instance.CurrentLevel.wavesToWin} waves!";
        objectiveText.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        objectiveText.gameObject.SetActive(false);
    }

    private void ShowMissionComplete()
    {
        UpdateNextLevelButton();
        missionCompletePanel.SetActive(true);
        GameManager_map_4.Instance.SetTimeScale(0f);
    }

    public void EnterEndlessMode()
    {
        missionCompletePanel.SetActive(false);
        GameManager_map_4.Instance.SetTimeScale(GameManager_map_4.Instance.GameSpeed);
        Spawner_map_4.Instance.EnableEndlessMode();
    }

    private void HideUI()
    {
        HidePanels();
        waveText.gameObject.SetActive(false);
        livesText.gameObject.SetActive(false);
        resourcesText.gameObject.SetActive(false);
        warningText.gameObject.SetActive(false);

        speed1Button.gameObject.SetActive(false);
        speed2Button.gameObject.SetActive(false);
        speed3Button.gameObject.SetActive(false);
        pauseButton.gameObject.SetActive(false);
    }

    private void ShowUI()
    {
        waveText.gameObject.SetActive(true);
        livesText.gameObject.SetActive(true);
        resourcesText.gameObject.SetActive(true);

        speed1Button.gameObject.SetActive(true);
        speed2Button.gameObject.SetActive(true);
        speed3Button.gameObject.SetActive(true);
        pauseButton.gameObject.SetActive(true);
    }

    private void HidePanels()
    {
        pausePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        missionCompletePanel.SetActive(false);
        HeroPanel.SetActive(false);
        Platform_map_4.towerPanelOpen = false;
    }

    public void LoadNextLevel()
    {
        var levelManager = LevelManager.Instance;
        int currentIndex = Array.IndexOf(levelManager.allLevels, levelManager.CurrentLevel);
        int nextIndex = currentIndex + 1;
        if (nextIndex < levelManager.allLevels.Length)
        {
            missionCompletePanel.SetActive(false);
            levelManager.LoadLevel(levelManager.allLevels[nextIndex]);
        }
    }

    private void UpdateNextLevelButton()
    {
        var levelManager = LevelManager.Instance;
        int currentIndex = Array.IndexOf(levelManager.allLevels, levelManager.CurrentLevel);
        nextLevelButton.interactable = currentIndex + 1 < levelManager.allLevels.Length;
    }
    public void ToggleSound()
    {
        _isMuted = !_isMuted;
        AudioListener.volume = _isMuted ? 0f : 1f;

        if (soundButtonImage != null)
        {
            soundButtonImage.color = _isMuted ? soundOffColor : soundOnColor;
        }

        PlayerPrefs.SetInt("IsMuted", _isMuted ? 1 : 0);
    }
}