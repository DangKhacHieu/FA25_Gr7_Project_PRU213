using UnityEngine;
using UnityEngine.UI;
using System;

public class EnemyHealth : MonoBehaviour
{
    public event Action OnDamaged;
    public event Func<int, int> OnTakeDamage;
    [Header("Thưởng khi bị tiêu diệt")]
    public int goldReward = 10;

    [Header("Cấu hình máu")]
    public int maxHP = 220;
    private int currentHP;

    [Header("Thanh máu")]
    public GameObject healthBarPrefab;
    private Image healthFill;
    private GameObject healthBarInstance;

    [Header("Âm thanh khi chết")]
    public AudioClip deathSound;
    [Range(0f, 1f)] public float deathSoundVolume = 1f;

    private bool isDead = false;
    public event Action OnEnemyDie;

    void Start()
    {
        currentHP = maxHP;

        // Tạo thanh máu nếu có prefab
        if (healthBarPrefab != null)
        {
            healthBarInstance = Instantiate(
                healthBarPrefab,
                transform.position + Vector3.up * 0.01f,
                Quaternion.identity,
                transform
            );

            Transform fillTransform = healthBarInstance.transform.Find("Background/Fill");
            if (fillTransform != null)
                healthFill = fillTransform.GetComponent<Image>();
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        // 🔹 Cho phép các script khác (như ValkyrieSkill) chỉnh damage
        if (OnTakeDamage != null)
            damage = OnTakeDamage.Invoke(damage);

        currentHP -= damage;
        if (currentHP < 0) currentHP = 0;
        UpdateHealthBar();

        OnDamaged?.Invoke(); // 🔔 THÊM DÒNG NÀY

        if (currentHP <= 0)
            Die();
    }


    void UpdateHealthBar()
    {
        if (healthFill != null)
            healthFill.fillAmount = (float)currentHP / maxHP;
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        OnEnemyDie?.Invoke();

        Animator anim = GetComponent<Animator>();
        if (anim != null)
            anim.SetTrigger("Die");

        // ✅ Tự phát âm thanh chết trên chính enemy này
        if (deathSound != null)
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = deathSound;
            audioSource.volume = deathSoundVolume;
            audioSource.spatialBlend = 0f;   // 2D sound, nghe toàn màn hình
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            audioSource.Play();
            Destroy(audioSource, deathSound.length + 0.1f);

            Debug.Log($"🎵 Phát âm thanh chết của {name}");
        }
        else
        {
            Debug.LogWarning($"⚠️ Không có deathSound gán cho {name}");
        }

        if (GoldManager.Instance != null)
            GoldManager.Instance.AddGold(goldReward);

        // Xoá enemy sau khi hoàn thành animation
        Destroy(gameObject, 0.8f);
    }
    public float GetCurrentHPPercent()
    {
        return (float)currentHP / maxHP;
    }


}
