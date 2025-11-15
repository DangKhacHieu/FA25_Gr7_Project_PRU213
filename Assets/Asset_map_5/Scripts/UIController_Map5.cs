using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController_Map5 : MonoBehaviour
{
    [SerializeField] private TMP_Text waveText;
    [SerializeField] private TMP_Text livesText;
    [SerializeField] private TMP_Text resourcesText;
    [SerializeField] private TMP_Text warningText;

    [SerializeField] private GameObject towerPanel;
    [SerializeField] private GameObject towerCardPrefab;
    [SerializeField] private Transform cardsContainer;

    [SerializeField] private HeroData_Map5[] towers;
    private List<GameObject> activeCards = new List<GameObject>();

    private Platform_Map5 _currentPlatform;

    [SerializeField] private Button speed1Button;
    [SerializeField] private Button speed2Button;
    [SerializeField] private Button speed3Button;
    [SerializeField] private Button muteButton;

    [SerializeField] private Color normalButtonColor = Color.white;
    [SerializeField] private Color selectedButtonColor = Color.blue;
    [SerializeField] private Color normalTextColor = Color.black;
    [SerializeField] private Color selectedTextColor = Color.white;

    [SerializeField] private GameObject pausePanel;
    private bool _isGamePaused = false;

    private bool _isMuted = false;


    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text objectiveText;
    [SerializeField] private GameObject missionCompletePanel;


    private void OnEnable()
    {
        Spawner_Map5.OnWaveChanged += UpdateWaveText;
        GameManager_Map5.OnLivesChanged += UpdateLivesText;
        GameManager_Map5.OnResourcesChanged += UpdateResourcesText;
        Platform_Map5.OnPlatformClicked += HandlePlatformClicked;
        HeroCard.OnTowerSelected += HandleTowerSelected;
        SceneManager.sceneLoaded += OnSceneLoaded;
        Spawner_Map5.OnMissionComplete += ShowMissionComplete;



    }

    private void OnDisable()
    {
        Spawner_Map5.OnWaveChanged -= UpdateWaveText;
        GameManager_Map5.OnLivesChanged -= UpdateLivesText;
        GameManager_Map5.OnResourcesChanged -= UpdateResourcesText;
        Platform_Map5.OnPlatformClicked -= HandlePlatformClicked;
        HeroCard.OnTowerSelected -= HandleTowerSelected;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Spawner_Map5.OnMissionComplete -= ShowMissionComplete;

    }

    private void Start()
    {
        speed1Button.onClick.AddListener(() => SetGameSpeed(0.2f));
        speed2Button.onClick.AddListener(() => SetGameSpeed(1f));
        speed3Button.onClick.AddListener(() => SetGameSpeed(2f));

        HighlightSelectedSpeedButton(GameManager_Map5.Instance.GameSpeed);
        // THÊM DÒNG NÀY: Gán sự kiện click cho nút Mute
        //muteButton.onClick.AddListener(ToggleMute);

        _isMuted = false; // Giả sử ban đầu game luôn bật tiếng
        AudioListener.volume = 1f; // 1f là âm lượng tối đa
        UpdateButtonVisual(muteButton, _isMuted); // Cập nhật màu cho nút mute
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
        waveText.text = $"Wave: {currentWave }";
    }

    private void UpdateLivesText(int currentLives)
    {
        livesText.text = $"{currentLives}";
        if (currentLives <= 0)
        {
            ShowGameOver();
        }

    }

    private void UpdateResourcesText(int currentResources)
    {
        resourcesText.text = $"{currentResources}";
    }
    private void HandlePlatformClicked(Platform_Map5 platform)
    {
        _currentPlatform = platform;

        ShowTowerPanel();
    }

    private void Awake()
    {
        // Tạo tất cả các card 1 lần duy nhất khi UIController khởi động
        PopulateInitialTowerCards();

        // Đảm bảo ban đầu panel bị ẩn
        towerPanel.SetActive(false);
    }

    private void PopulateInitialTowerCards()
    {
        foreach (var data in towers)
        {
            GameObject cardGameObject = Instantiate(towerCardPrefab, cardsContainer);
            HeroCard card = cardGameObject.GetComponent<HeroCard>();
            card.Initialize(data);
            activeCards.Add(cardGameObject);
            // Ban đầu ẩn card, chúng sẽ được hiển thị khi towerPanel.SetActive(true)
            cardGameObject.SetActive(true); // Hoặc false, nếu bạn muốn Panel cha quản lý việc hiển thị
        }
    }

    // Giữ nguyên hàm ShowTowerPanel(), vì giờ nó không cần gọi PopulateTowerCards() nữa
    private void ShowTowerPanel()
    {
        towerPanel.SetActive(true);
        Platform_Map5.towerPanelOpen = true;
        GameManager_Map5.Instance.SetTimeScale(0f);
        // BỎ CÁCH GỌI PopulateTowerCards() Ở ĐÂY!
    }

    // Giữ nguyên hàm HideTowerPanel()
    public void HideTowerPanel()
    {
        towerPanel.SetActive(false);
        Platform_Map5.towerPanelOpen = false;
        GameManager_Map5.Instance.SetTimeScale(GameManager_Map5.Instance.GameSpeed);
    }

    //private void PopulateTowerCards()
    //{
    //    foreach (var card in activeCards)
    //    {
    //        Destroy(card);
    //    }
    //    activeCards.Clear();

    //    foreach (var data in towers)
    //    {
    //        GameObject cardGameObject = Instantiate(towerCardPrefab, cardsContainer);
    //        HeroCard card = cardGameObject.GetComponent<HeroCard>();
    //        card.Initialize(data);
    //        activeCards.Add(cardGameObject);
    //    }
    //}

    private void HandleTowerSelected(HeroData_Map5 towerData)
    {
        if (_currentPlatform.transform.childCount > 0)
        {
            HideTowerPanel();
            StartCoroutine(ShowWarningMessage("This platform already has a tower!"));
            return;
        }
        if (GameManager_Map5.Instance.Resources >= towerData.cost)
        {
            GameManager_Map5.Instance.SpendResources(towerData.cost);
            _currentPlatform.PlaceTower(towerData);
        }
        else
        {
            StartCoroutine(ShowWarningMessage("Not enough resources!"));

        }

        HideTowerPanel();

    }

    private IEnumerator ShowWarningMessage(string message)
    {
        warningText.text = message;
        warningText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(3f);
        warningText.gameObject.SetActive(false);

    }

    private void SetGameSpeed(float timeScale)
    {
        HighlightSelectedSpeedButton(timeScale);
        GameManager_Map5.Instance.SetGameSpeed(timeScale);
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
        if (towerPanel.activeSelf)
            return;

        if (_isGamePaused)
        {
            pausePanel.SetActive(false);
            _isGamePaused = false;
            GameManager_Map5.Instance.SetTimeScale(GameManager_Map5.Instance.GameSpeed);
        }
        else
        {
            pausePanel.SetActive(true);
            _isGamePaused = true;
            GameManager_Map5.Instance.SetTimeScale(0f);
        }
    }

    public void ToggleMute()
    {
        // 1. Đảo ngược trạng thái (đang tắt -> bật, đang bật -> tắt)
        _isMuted = !_isMuted;

        // 2. Cập nhật âm lượng tổng của game
        if (_isMuted)
        {
            AudioListener.volume = 0f; // Tắt tiếng
        }
        else
        {
            AudioListener.volume = 1f; // Bật tiếng
        }

        // 3. Cập nhật màu sắc của nút
        // Chúng ta tận dụng hàm UpdateButtonVisual bạn đã có.
        // Khi _isMuted = true (đã tắt), nút sẽ có màu "selected" (màu xanh)
        UpdateButtonVisual(muteButton, _isMuted);
    }

    public void RestartLevel()
    {
        LevelManager.Instance.LoadLevel(LevelManager.Instance.CurrentLevel);

    }

    public void QuitGame()
    {
    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
    #else
                        Application.Quit();
    #endif
    }

    public void GoToMainMenu()
    {
        GameManager_Map5.Instance.SetTimeScale(1f);
        SceneManager.LoadScene("MainMenu");
    }

    private void ShowGameOver()
    {
        GameManager_Map5.Instance.SetTimeScale(0f);
        gameOverPanel.SetActive(true);
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(ShowObjective());
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
        missionCompletePanel.SetActive(true);
        GameManager_Map5.Instance.SetTimeScale(0f);
    }

    public void EnterEndlessMode()
    {
        missionCompletePanel.SetActive(false);
        GameManager_Map5.Instance.SetTimeScale(GameManager_Map5.Instance.GameSpeed);
        Spawner_Map5.Instance.EnableEndlessMode();
    }

}
