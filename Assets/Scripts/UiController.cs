using TMPro;
using UnityEngine;

public class UiController : MonoBehaviour
{
    [SerializeField] private TMP_Text waveText;
    [SerializeField] private TMP_Text LiveText;

    private void OnEnable()
    {
        Spawner.OnWaveChange += UpdateWaveText;
        GameManager.OnLivesChanged += UpdateLiveText;
    }

    private void DisEnable()
    {
        Spawner.OnWaveChange -= UpdateWaveText;
        GameManager.OnLivesChanged -= UpdateLiveText;

    }

    private void UpdateWaveText(int currentWave)
    {
        waveText.text = $"wave: {currentWave++}";
    }

    private void UpdateLiveText(int currentLives)
    {
        LiveText.text = $"Live: {currentLives}";
    }
}
