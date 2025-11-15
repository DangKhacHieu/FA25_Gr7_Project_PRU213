using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GateShake : MonoBehaviour
{
    [Header("Shake Settings")]
    public float shakeDuration = 0.5f;   // thời gian rung
    public float shakeMagnitude = 0.05f; // biên độ rung

    [Header("Sound Settings")]
    public AudioClip shakeSound;         // file âm thanh (rung, va chạm)
    [Range(0f, 1f)]
    public float soundVolume = 0.7f;

    private Vector3 originalPos;
    private AudioSource audioSource;

    void Start()
    {
        originalPos = transform.localPosition;

        // Chuẩn bị AudioSource
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public void ShakeGate()
    {
        StopAllCoroutines(); // không để rung chồng
        StartCoroutine(DoShake());

        // 🔊 phát âm thanh nếu có file
        if (shakeSound != null)
        {
            audioSource.PlayOneShot(shakeSound, soundVolume);
        }
    }

    private System.Collections.IEnumerator DoShake()
    {
        float elapsed = 0.0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            transform.localPosition = originalPos + new Vector3(x, y, 0);
            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPos; // trả lại vị trí gốc
    }
}
