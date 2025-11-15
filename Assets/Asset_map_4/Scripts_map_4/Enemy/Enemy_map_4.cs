using System;
using UnityEngine;

public class Enemy_map_4 : MonoBehaviour
{
    [Header("Data & Components")]
    [SerializeField] private EnemyData_map_4 data;
    public EnemyData_map_4 Data => data;

    [SerializeField] protected Animator animator;

    protected Path_map_4 _currentPath;
    protected Vector3 _targetPosition;
    protected int _currentWaypoint;
    protected float _lives;
    protected float _maxLives;
    protected bool _hasBeenCounted = false;
    protected bool _facingRight = true;
    protected bool _isDead = false;
    protected GameObject isMark;

    // 🌟 THÊM MỚI: Biến tốc độ riêng cho từng con quái
    protected float _currentSpeed;

    public float CurrentHP => _lives;

    protected HeathBar _heathBar;
    private SpriteRenderer spriteRenderer;

    public static event Action<EnemyData_map_4> OnEnemyReachedEnd;
    public static event Action<Enemy_map_4> OnEnemyDestroyed;

    protected virtual void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _heathBar = GetComponentInChildren<HeathBar>();
    }

    private void OnEnable()
    {
        animator?.SetBool("isMoving", true);
    }

    protected virtual void Update()
    {
        if (_isDead || _hasBeenCounted) return;
        if (_currentPath == null || _currentPath.Waypoints.Length == 0) return;
        MoveAlongPath();
    }

    protected virtual void MoveAlongPath()
    {
        // 🌟 THAY ĐỔI: Dùng _currentSpeed thay vì data.speed
        float step = _currentSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, step);

        Vector3 dir = _targetPosition - transform.position;
        if (Mathf.Abs(dir.x) > 0.01f)
            FlipSprite(dir.x > 0);

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
                _hasBeenCounted = true;
                animator?.SetBool("isMoving", false);
                OnEnemyReachedEnd?.Invoke(data);
                gameObject.SetActive(false);
            }
        }
    }
    protected virtual void FlipSprite(bool movingRight)
    {
        if (movingRight && !_facingRight || !movingRight && _facingRight)
        {
            _facingRight = !_facingRight;
            Vector3 scale = transform.localScale;
            scale.x = data.artFacesRight ? Mathf.Abs(scale.x) * (_facingRight ? 1 : -1)
                                    : Mathf.Abs(scale.x) * (_facingRight ? -1 : 1);
            transform.localScale = scale;
        }
    }

    public virtual void TakeDamage(float damage)
    {
        if (_isDead || _hasBeenCounted) return;

        _lives -= damage;
        _lives = Mathf.Clamp(_lives, 0, _maxLives);

        _heathBar.UpdateHeathBar(_maxLives, _lives);

        if (_lives <= 0)
        {
            Die();
        }
        else
        {
            animator?.SetTrigger("Hurt");
        }
    }

    protected virtual void Die()
    {
        _isDead = true;
        animator?.SetTrigger("Die");
        OnEnemyDestroyed?.Invoke(this);
        StartCoroutine(HandleDeath());
    }

    protected virtual System.Collections.IEnumerator HandleDeath()
    {
        yield return new WaitForSeconds(1.2f);
        gameObject.SetActive(false);
    }

    public virtual void Initialize(float healthMultiplier, Path_map_4 assignedPath)
    {
        _hasBeenCounted = false;
        _maxLives = data.lives * healthMultiplier;
        _lives = _maxLives;
        _isDead = false;

        // 🌟 THÊM MỚI: Khởi tạo tốc độ ban đầu
        _currentSpeed = data.speed;

        _currentPath = assignedPath;
        _currentWaypoint = 0;

        if (_currentPath != null && _currentPath.Waypoints.Length > 0)
            _targetPosition = _currentPath.GetPosition(_currentWaypoint);

        _heathBar.UpdateHeathBar(_maxLives, _lives);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        _isDead = false;
    }
}