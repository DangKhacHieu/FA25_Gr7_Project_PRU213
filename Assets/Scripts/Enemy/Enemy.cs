using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Data & Components")]
    [SerializeField] private EnemyData data;
    public EnemyData Data => data;

    [SerializeField] private Animator animator;
    [SerializeField] private Transform healthBar;

    private Path _currentPath;
    private Vector3 _targetPosition;
    private int _currentWaypoint;

    private float _lives;
    public float CurrentHP => _lives;
    private float _maxLives;
    private bool _hasBeenCounted = false;
    private bool _facingRight = true;
    private bool _isDead = false;

    private Vector3 _healthBarOriginalScale;
    private SpriteRenderer spriteRenderer;

    [Header("Facing Settings")]
    [Tooltip("Nếu sprite gốc nhìn sang phải thì bật = true, nhìn trái thì tắt")]
    [SerializeField] private bool artFacesRight = true;

    public static event Action<EnemyData> OnEnemyReachedEnd;
    public static event Action<Enemy> OnEnemyDestroyed;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _healthBarOriginalScale = healthBar.localScale;
    }

    private void OnEnable()
    {
        // Path sẽ được gán từ Spawner qua Initialize()
        animator?.SetBool("isMoving", true);
    }

    private void Update()
    {
        if (_isDead || _hasBeenCounted) return;
        if (_currentPath == null || _currentPath.Waypoints.Length == 0) return;

        MoveAlongPath();
    }

    private void MoveAlongPath()
    {
        float step = data.speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, step);

        // Xoay sprite theo hướng đi
        Vector3 dir = _targetPosition - transform.position;
        if (Mathf.Abs(dir.x) > 0.01f)
            FlipSprite(dir.x > 0);

        // Check đến waypoint tiếp theo
        float distance = Vector3.Distance(transform.position, _targetPosition);
        if (distance < 0.1f)
        {
            if (_currentWaypoint < _currentPath.Waypoints.Length - 1)
            {
                _currentWaypoint++;
                _targetPosition = _currentPath.GetPosition(_currentWaypoint);
            }
            else
            {
                // Đến cuối đường
                _hasBeenCounted = true;
                animator?.SetBool("isMoving", false);
                OnEnemyReachedEnd?.Invoke(data);
                gameObject.SetActive(false);
            }
        }
    }

    private void FlipSprite(bool movingRight)
    {
        if (movingRight && !_facingRight || !movingRight && _facingRight)
        {
            _facingRight = !_facingRight;
            Vector3 scale = transform.localScale;
            scale.x = artFacesRight ? Mathf.Abs(scale.x) * (_facingRight ? 1 : -1)
                                    : Mathf.Abs(scale.x) * (_facingRight ? -1 : 1);
            transform.localScale = scale;
        }
    }

    public void TakeDamage(float damage)
    {
        if (_isDead || _hasBeenCounted) return;

        _lives -= damage;
        _lives = Mathf.Clamp(_lives, 0, _maxLives);
        UpdateHealthBar();

        if (_lives <= 0)
        {
            Die();
        }
        else
        {
            animator?.SetTrigger("Hurt");
        }
    }

    private void Die()
    {
        _isDead = true;
        animator?.SetTrigger("Die");
        OnEnemyDestroyed?.Invoke(this);
        StartCoroutine(HandleDeath());
    }

    private System.Collections.IEnumerator HandleDeath()
    {
        yield return new WaitForSeconds(1.2f);
        gameObject.SetActive(false);
    }

    private void UpdateHealthBar()
    {
        if (_maxLives <= 0f)
            _maxLives = 1f;

        float healthPercent = Mathf.Clamp01(_lives / _maxLives);
        if (float.IsNaN(healthPercent))
            healthPercent = 0f;

        Vector3 scale = _healthBarOriginalScale;
        scale.x = _healthBarOriginalScale.x * healthPercent;
        healthBar.localScale = scale;
    }

    /// <summary>
    /// Khởi tạo enemy với Path và chỉ số máu cụ thể từ Spawner.
    /// </summary>
    public void Initialize(float healthMultiplier, Path assignedPath)
    {
        _hasBeenCounted = false;
        _maxLives = data.lives * healthMultiplier;
        _lives = _maxLives;
        _isDead = false;

        _currentPath = assignedPath;
        _currentWaypoint = 0;

        if (_currentPath != null && _currentPath.Waypoints.Length > 0)
            _targetPosition = _currentPath.GetPosition(_currentWaypoint);

        UpdateHealthBar();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        _isDead = false;
    }
}
