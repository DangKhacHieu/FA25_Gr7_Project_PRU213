using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("SFX")]
    [Range(0f, 1f)] public float sfxVolume = 1f;

    private AudioSource sfxSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
            sfxSource.loop = false;
            sfxSource.spatialBlend = 0f;   // 2D sound để không bị mất tiếng theo khoảng cách
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ✅ PlayOneShot không cần destroy gì cả, Unity tự xử lý
    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;

        // PlayOneShot tự chồng nhiều âm thanh mà không destroy object
        sfxSource.PlayOneShot(clip, Mathf.Clamp01(volume) * sfxVolume);
    }
}
