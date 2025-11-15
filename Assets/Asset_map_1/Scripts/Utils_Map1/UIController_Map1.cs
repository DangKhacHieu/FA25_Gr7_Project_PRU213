using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIController_Map1 : MonoBehaviour
{
    [SerializeField] private TMP_Text waveText;
    [SerializeField] private TMP_Text livesText;
    [SerializeField] private TMP_Text resourcesText;
    [SerializeField] private TMP_Text warningText;

    [SerializeField] private GameObject HeroPanel;
    [SerializeField] private GameObject HeroCardPrefab;
    [SerializeField] private Transform cardsContainer;

    [SerializeField] private HeroData_Map1[] hero;
    private List<GameObject> activeCards = new List<GameObject>();

    private Platform_Map1 _currentPlatform;

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
        Spawner_Map1.OnWaveChanged += UpdateWaveText;
        GameManager_Map1.OnLivesChanged += UpdateLivesText;
        GameManager_Map1.OnResourcesChanged += UpdateResourcesText;
        Platform_Map1.OnPlatformClicked += HandlePlatformClicked;
        HeroCard_Map1.OnTowerSelected += HandleHeroSelected;
        SceneManager.sceneLoaded += OnSceneLoaded;
        Spawner_Map1.OnMissionComplete += ShowMissionComplete;
    }

    private void OnDisable()
    {
        Spawner_Map1.OnWaveChanged -= UpdateWaveText;
        GameManager_Map1.OnLivesChanged -= UpdateLivesText;
        GameManager_Map1.OnResourcesChanged -= UpdateResourcesText;
        Platform_Map1.OnPlatformClicked -= HandlePlatformClicked;
        HeroCard_Map1.OnTowerSelected -= HandleHeroSelected;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Spawner_Map1.OnMissionComplete -= ShowMissionComplete;
    }

    private void Start()
    {
        speed1Button.onClick.AddListener(() => SetGameSpeed(0.2f));
        speed2Button.onClick.AddListener(() => SetGameSpeed(1f));
        speed3Button.onClick.AddListener(() => SetGameSpeed(2f));

        HighlightSelectedSpeedButton(GameManager_Map1.Instance.GameSpeed);

        // THÊM DÒNG NÀY: Gán sự kiện click cho nút Mute
        //muteButton.onClick.AddListener(ToggleMute);

        _isMuted = false; 
        AudioListener.volume = 1f; 
        UpdateButtonVisual(muteButton, _isMuted); 
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
        waveText.text = $"Wave: {currentWave + 1}";
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


    private void HandlePlatformClicked(Platform_Map1 platform)
    {
        _currentPlatform = platform;
        ShowHeroPanel();
    }

    private void ShowHeroPanel()
    {
        HeroPanel.SetActive(true);
        Platform_Map1.heroPanelOpen = true;
        GameManager_Map1.Instance.SetTimeScale(0f);
        PopulateHeroCards();
    }

    public void HideHeroPanel()
    {
        HeroPanel.SetActive(false);
        Platform_Map1.heroPanelOpen = false;
        GameManager_Map1.Instance.SetTimeScale(GameManager_Map1.Instance.GameSpeed);

    }

    private void PopulateHeroCards()
    {
        foreach (var card in activeCards)
        {
            Destroy(card);
        }
        activeCards.Clear();

        foreach (var data in hero)
        {
            GameObject cardGameObject = Instantiate(HeroCardPrefab, cardsContainer);
            HeroCard_Map1 card = cardGameObject.GetComponent<HeroCard_Map1>();
            card.Initialize(data);
            activeCards.Add(cardGameObject);
        }
    }

    private void HandleHeroSelected(HeroData_Map1 heroData)
    {
        if (_currentPlatform.transform.childCount > 0)
        {
            HideHeroPanel();
            StartCoroutine(ShowWarningMessage("This platform already has a Hero!"));
            return;
        }

        if (GameManager_Map1.Instance.Resources >= heroData.cost)
        {
            GameManager_Map1.Instance.SpendResources(heroData.cost);
            _currentPlatform.PlaceHero(heroData);
        }
        else
        {
            StartCoroutine(ShowWarningMessage("Not Enough Resources"));
        }

        HideHeroPanel();

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
        GameManager_Map1.Instance.SetGameSpeed(timeScale);
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
        if (HeroPanel.activeSelf)
            return;

        if (_isGamePaused)
        {
            pausePanel.SetActive(false);
            _isGamePaused = false;
            GameManager_Map1.Instance.SetTimeScale(GameManager_Map1.Instance.GameSpeed);
        }
        else
        {
            pausePanel.SetActive(true);
            _isGamePaused = true;
            GameManager_Map1.Instance.SetTimeScale(0f);
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
        GameManager_Map1.Instance.SetTimeScale(1f);
        SceneManager.LoadScene("MainMenu");
    }

    private void ShowGameOver()
    {
        GameManager_Map1.Instance.SetTimeScale(0f);
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
        GameManager_Map1.Instance.SetTimeScale(0f);
    }

    public void EnterEndlessMode()
    {
        missionCompletePanel.SetActive(false);
        GameManager_Map1.Instance.SetTimeScale(GameManager_Map1.Instance.GameSpeed);
        Spawner_Map1.Instance.EnableEndlessMode();
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


}

