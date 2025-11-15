using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class HeroAttack_map_4 : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private HeroData heroData;

    [Header("References")]
    public Transform throwPoint;
    public ObjectPooler_map_4 spearPool;
    public Animator animator;
    public GameObject Stun_effect;

    [Header("Audio")]
    [SerializeField] private AudioClip soundAttack;
    [Range(0f, 1f)][SerializeField] private float AttackVolume = 1f;
    private AudioSource audioSource;

    private Transform currentTarget;
    private float nextAttackTime;
    private bool facingRight = true;
    private bool _isStun = false;
    private bool _isAttacking = false;

    private int attackCounter = 0;

    private void Awake()
    {
        // ✅ Tự động tạo AudioSource nếu chưa có
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f; // game 2D
    }

    private void Update()
    {
        if (heroData == null) return;

        // --- 1. STUN ---
        if (_isStun)
        {
            if (Time.time < nextAttackTime) return;
            _isStun = false;
            Stun_effect.SetActive(false);
        }

        // --- 2. Kiểm tra target ---
        if (currentTarget != null)
        {
            bool invalid = !currentTarget.gameObject.activeInHierarchy ||
                           Vector2.Distance(transform.position, currentTarget.position) > heroData.attackRange;
            if (invalid)
                currentTarget = null;
        }

        // --- 3. Tìm mục tiêu mới ---
        if (currentTarget == null)
            FindNewTarget();

        _isAttacking = (currentTarget != null);

        // --- 4. Tấn công ---
        if (_isAttacking && Time.time >= nextAttackTime)
        {
            AttackTarget(currentTarget);
            nextAttackTime = Time.time + heroData.attackCooldown;
        }

        // --- 5. Quay mặt ---
        if (currentTarget != null)
            UpdateFacing(currentTarget.position.x);
    }

    private void FindNewTarget()
    {
        var enemies = Physics2D.OverlapCircleAll(transform.position, heroData.attackRange, heroData.enemyLayer);
        var valid = enemies
            .Select(c => c.GetComponent<Enemy_map_4>())
            .Where(e => e != null && e.gameObject.activeInHierarchy && e.CurrentHP > 0)
            .OrderBy(e => Vector2.Distance(e.transform.position, transform.position))
            .ToList();

        currentTarget = valid.FirstOrDefault()?.transform;
    }

    private void AttackTarget(Transform target)
    {
        animator?.SetTrigger("Attack");
        attackCounter++;
    }

    // Gọi từ Animation Event hoặc Attack Trigger
    public void LaunchProjectile()
    {
        if (currentTarget == null || !currentTarget.gameObject.activeInHierarchy)
            return;

        // ✅ Phát âm thanh khi bắn
        if (soundAttack != null)
            audioSource.PlayOneShot(soundAttack, AttackVolume);

        // Lấy đạn từ pool
        var spear = spearPool.GetPooledObject();
        if (spear == null) return;
        spear.transform.position = throwPoint.position;
        spear.SetActive(true);

        float currentDamage = heroData.projectileDamage;
        float currentLifetime = heroData.projectileLifeTime;
        float currentSpeed = heroData.projectileSpeed;
        bool isSpecial = false;

        if (heroData.hasEmpoweredAttack && attackCounter % heroData.attacksForSpecial == 0)
        {
            Debug.Log($"Hero {heroData.displayName}: NGỌN LAO PHÁN QUYẾT!");
            currentDamage *= heroData.specialDamageMultiplier;
            currentLifetime *= heroData.specialLifetimeMultiplier;
            isSpecial = true;
        }

        var proj = spear.GetComponent<Weapon_Projectile_map_4>();
        if (proj != null)
        {
            proj.Launch(currentTarget,
                        currentSpeed,
                        currentDamage,
                        currentLifetime,
                        isSpecial,
                        heroData.specialEffectPrefab);
        }
    }

    private void UpdateFacing(float targetX)
    {
        bool targetIsRight = targetX > transform.position.x;
        if (targetIsRight != facingRight)
        {
            facingRight = targetIsRight;
            var scale = transform.localScale;
            scale.x = heroData.artFacesRight
                ? Mathf.Abs(scale.x) * (facingRight ? 1 : -1)
                : Mathf.Abs(scale.x) * (facingRight ? -1 : 1);
            transform.localScale = scale;
        }
    }

    public void Stun(float duration)
    {
        _isStun = true;
        Stun_effect.SetActive(true);
        currentTarget = null;
        _isAttacking = false;
        nextAttackTime = Time.time + duration;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (heroData == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, heroData.attackRange);
    }
#endif
}
