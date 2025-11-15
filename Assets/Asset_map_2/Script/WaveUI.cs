using TMPro;
using UnityEngine;

public class WaveUI : MonoBehaviour
{
    public static WaveUI Instance;
    public TextMeshProUGUI waveText;

    void Awake()
    {
        Instance = this;
    }

    public void UpdateWaveText(int waveNumber)
    {
        if (waveText != null)
            waveText.text = "Wave: " + waveNumber;
    }
}
