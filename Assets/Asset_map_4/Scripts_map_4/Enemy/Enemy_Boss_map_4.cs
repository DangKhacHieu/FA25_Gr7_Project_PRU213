using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public enum BossState
{
    Walking,
    Roaring,
    Attacking,
    Idle
}

public class Enemy_Boss_map_4 : Enemy_map_4
{
    [Header("Boss Logic")]
    [SerializeField] private GameObject firePrefab;
    [SerializeField] private LayerMask heroLayer;
    [SerializeField] private LayerMask platformLayer;
    [SerializeField] private GameObject ThrowPoint;

    [Header("Ground State (Walk/Roar)")]
    [SerializeField] private float walkDuration = 10f;
    [SerializeField] private float roarDuration = 2.5f;
    [SerializeField] private float roarRange = 5f;
    [SerializeField] private float roarStunDuration = 3f;

    [Header("Combat")]
    [Tooltip("Thời gian hồi chiêu sau khi phun lửa (giây)")]
    [SerializeField] private float attackCooldown = 4f;

    [Header("Audio")]
    [SerializeField] private AudioClip roarSound;
    [Range(0f, 1f)][SerializeField] private float roarVolume = 1f;
    private AudioSource audioSource;

    private BossState _currentState = BossState.Idle;
    private bool _isInvulnerable = false;
    private float _stateTimer = 0f;

    private int _currentPhase = 0;
    private float _phaseThreshold75;
    private float _phaseThreshold50;
    private float _phaseThreshold25;

    // FIX: Đã xóa 'private HeathBar _heathBar;'
    // vì đã được thừa hưởng từ Enemy_map_4

    public override void Initialize(float healthMultiplier, Path_map_4 assignedPath)
    {
        // base.Initialize SẼ TỰ ĐỘNG GỌI _heathBar.UpdateHeathBar
        base.Initialize(healthMultiplier, assignedPath);

        _phaseThreshold75 = _maxLives * 0.75f;
        _phaseThreshold50 = _maxLives * 0.50f;
        _phaseThreshold25 = _maxLives * 0.25f;
        _currentPhase = 0;

        // Đảm bảo AudioSource tồn tại
        if (audioSource == null)
        {
            audioSource = gameObject.GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        ChangeState(BossState.Walking);
    }

    protected override void Update()
    {
        if (_isDead) return;

        if (_stateTimer > 0)
            _stateTimer -= Time.deltaTime;

        switch (_currentState)
        {
            case BossState.Idle:
                if (_stateTimer <= 0)
                    ChangeState(BossState.Walking);
                break;

            case BossState.Walking:
                base.MoveAlongPath();
                if (_stateTimer <= 0)
                    ChangeState(BossState.Roaring);
                break;

            case BossState.Roaring:
                if (_stateTimer <= 0)
                    ChangeState(BossState.Walking);
                break;

            case BossState.Attacking:
                if (_stateTimer <= 0)
                    ChangeState(BossState.Walking);
                break;
        }
    }

    public override void TakeDamage(float damage)
    {
        if (_isInvulnerable || _isDead) return;

        _lives -= damage;
        _lives = Mathf.Clamp(_lives, 0, _maxLives);

        _heathBar.UpdateHeathBar(_maxLives, _lives);

        CheckForPhaseChange();

        if (_lives <= 0)
        {
            Die();
        }
    }

    private void CheckForPhaseChange()
    {
        if (_currentPhase == 0 && _lives <= _phaseThreshold75)
        {
            _currentPhase = 1;
            ChangeState(BossState.Attacking);
        }
        else if (_currentPhase == 1 && _lives <= _phaseThreshold50)
        {
            _currentPhase = 2;
            ChangeState(BossState.Attacking);
        }
        else if (_currentPhase == 2 && _lives <= _phaseThreshold25)
        {
            _currentPhase = 3;
            ChangeState(BossState.Attacking);
        }
    }

    private void ChangeState(BossState newState)
    {
        if (_currentState == BossState.Attacking && newState == BossState.Roaring)
            return;

        _currentState = newState;
        Debug.Log($"Boss changing state to: {newState}");

        switch (newState)
        {
            case BossState.Idle:
                animator.SetTrigger("idle");
                _stateTimer = 2f;
                _isInvulnerable = false;
                break;

            case BossState.Walking:
                animator.SetTrigger("walk");
                if (_stateTimer <= 0)
                    _stateTimer = walkDuration;
                _isInvulnerable = false;
                break;

            case BossState.Roaring:
                animator.SetTrigger("roar");
                _stateTimer = roarDuration;
                _isInvulnerable = true;
                DoRoar();
                break;

            case BossState.Attacking:
                animator.SetTrigger("attack");
                _stateTimer = attackCooldown;
                _isInvulnerable = true;
                AttackTargetPlatform();
                break;
        }
    }
    private void DoRoar()
    {
        Debug.Log("BOSS ROARS!");

        // Stun hero xung quanh
        Collider2D[] heroes = Physics2D.OverlapCircleAll(transform.position, roarRange, heroLayer);
        foreach (var heroCollider in heroes)
        {
            var heroAttack = heroCollider.GetComponentInChildren<HeroAttack_map_4>();
            if (heroAttack != null)
            {
                heroAttack.Stun(roarStunDuration);
            }
        }

        // 🔊 phát âm thanh gầm (nếu có)
        if (roarSound != null && audioSource != null)
        {
            audioSource.Stop();
            audioSource.clip = roarSound;
            audioSource.volume = roarVolume;
            audioSource.Play();
        }
    }

    private void AttackTargetPlatform()
    {
        if (firePrefab == null)
        {
            Debug.LogWarning("[Boss] firePrefab chưa gán!");
            return;
        }

        // Tìm tất cả platform trong scene có chứa Hero (có HeroAttack_map_4 trong children) và nằm trong roarRange
        var targets = FindPlatformsWithHeroWithinRange();

        if (targets == null || targets.Count == 0)
        {
            Debug.LogWarning("⚠️ Boss muốn tấn công nhưng không có platform nào chứa hero trong roarRange!");
            return;
        }

        Vector3 fireOrigin = ThrowPoint.transform.position;

        foreach (var platform in targets)
        {
            Vector3 targetPos = platform.transform.position;

            for (int i = -1; i <= 1; i++)
            {
                Vector3 offset = Vector3.right * i * 1.5f; // khoảng cách lệch ngang
                GameObject fireballObj = Instantiate(firePrefab, fireOrigin, Quaternion.identity);

                var fireball = fireballObj.GetComponent<BossFireball_map_4>();
                if (fireball != null)
                {
                    // Truyền cả Platform reference để fireball không dò lại
                    fireball.Launch(targetPos, platform);
                }
            }

            Debug.Log($"🔥 Boss phun 3 quả vào platform chứa hero: {platform.name}");
        }
    }

    private List<Platform_map_4> FindPlatformsWithHeroWithinRange()
    {
        var result = new List<Platform_map_4>();

        // Lấy tất cả platform hiện có
        var allPlatforms = Object.FindObjectsByType<Platform_map_4>(FindObjectsSortMode.None);

        foreach (var p in allPlatforms)
        {
            float dist = Vector2.Distance(transform.position, p.transform.position);
            if (dist > roarRange) continue;

            // Kiểm tra platform có chứa hero (child) hay không
            if (p.GetComponentInChildren<HeroAttack_map_4>() != null)
            {
                result.Add(p);
            }
        }

        return result;
    }



    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, roarRange);
    }
}