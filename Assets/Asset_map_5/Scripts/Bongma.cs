using System;
using System.Collections;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class Bongma : MonoBehaviour
{
    [SerializeField] private EnemyData_Map5 data;
    public EnemyData_Map5 Data => data;

    public static event Action<EnemyData_Map5> OnEnemyReachedEnd;
    public static event Action<Bongma> OnEnemyDestroyed;
    public static event Action<float> OnEnemyDestroyedWithReward;

    // SỰ KIỆN CHO CÁC KỸ NĂNG LẮNG NGHE
    public event Action<Bongma> OnSpawn;
    public event Action<Bongma> OnHealthLow;
    public event Action<Bongma, float> OnBeforeTakeDamage;
    public event Action<Bongma> OnDeath;

    private bool _hasFiredLowHealthEvent = false;

    private Path _path;

    private Vector3 _targetPosition;
    private int _currentWaypoint;
    private float _lives;
    private float _maxLives;

    // BIẾN CỤC BỘ: Theo dõi tốc độ runtime (không phải của Scriptable Object)
    private float _currentSpeed;

    private Animator _animator;

    [SerializeField] private Transform healthBar;
    private Vector3 _healthBarOriginalScale;
    private bool _hasBeenCounted = false;


    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _healthBarOriginalScale = healthBar.localScale;
    }

    private void OnEnable()
    {
        _currentWaypoint = 0;

        if (_animator != null)
        {
            _animator.SetBool("die", false);
        }

        if (_path != null)
        {
            _targetPosition = _path.GetPosition(_currentWaypoint);
        }
    }

    public void SetPath(Path path)
    {
        _path = path;
        _currentWaypoint = 0;
        if (_path != null && _path.Waypoints.Length > 0)
        {
            _targetPosition = _path.GetPosition(_currentWaypoint);
        }
    }

    void Update()
    {
        if (_hasBeenCounted || _path == null) return;

        // SỬ DỤNG TỐC ĐỘ CỤC BỘ
        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, _currentSpeed * Time.deltaTime);

        // KIỂM TRA MÁU THẤP
        if (!_hasFiredLowHealthEvent && _lives > 0 && _lives <= _maxLives * 0.3f)
        {
            OnHealthLow?.Invoke(this);
            _hasFiredLowHealthEvent = true;
        }

        float relativeDistance = (transform.position - _targetPosition).magnitude;
        if (relativeDistance < 0.1f)
        {
            if (_currentWaypoint < _path.Waypoints.Length - 1)
            {
                _currentWaypoint++;
                _targetPosition = _path.GetPosition(_currentWaypoint);
            }
            else // reached last waypoint
            {
                _hasBeenCounted = true;
                OnEnemyReachedEnd?.Invoke(data);
                gameObject.SetActive(false);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (_hasBeenCounted) return;

        OnBeforeTakeDamage?.Invoke(this, damage);

        _lives -= damage;

        // Xử lý Giới Hạn Máu Tối Đa và Tối Thiểu
        _lives = Mathf.Min(_lives, _maxLives);
        _lives = Mathf.Max(_lives, 0);

        UpdateHealthBar();

        if (_lives <= 0)
        {
            OnDeath?.Invoke(this);

            _hasBeenCounted = true;

            if (_animator != null)
            {
                _animator.SetBool("die", true);
            }
            StartCoroutine(DieSequence(1f));
        }
    }

    private IEnumerator DieSequence(float delay)
    {
        yield return new WaitForSeconds(delay);
        OnEnemyDestroyed?.Invoke(this);
        gameObject.SetActive(false);
    }


    private void UpdateHealthBar()
    {
        float healthPercent = _lives / _maxLives;
        Vector3 scale = _healthBarOriginalScale;
        scale.x = _healthBarOriginalScale.x * healthPercent;
        healthBar.localScale = scale;
    }

    public void Initialize(float healthMultiplier)
    {
        _hasBeenCounted = false;
        _maxLives = data.lives * healthMultiplier;

        _lives = _maxLives;
        _hasFiredLowHealthEvent = false;
        UpdateHealthBar();

        // QUAN TRỌNG: Khởi tạo tốc độ cục bộ từ dữ liệu gốc
        _currentSpeed = data.speed;

        OnSpawn?.Invoke(this);
    }

    // PHƯƠNG THỨC MỚI: Chỉ sửa đổi tốc độ cục bộ, KHÔNG sửa đổi EnemyData
    public void SetSpeed(float newSpeed)
    {
        _currentSpeed = newSpeed;
    }
}