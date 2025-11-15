using System.Collections.Generic;
using UnityEngine;
using System.Collections; // Cần thiết cho Coroutine
using System.Linq; // Cần thiết cho các thao tác List linh hoạt hơn

public class Hero : MonoBehaviour
{
    [SerializeField] private HeroData_Map5 data;
    private CircleCollider2D _circleCollider;

    private List<Bongma> _enemiesInRange = new List<Bongma>(); // Khởi tạo List ngay
    private ObjectPooler _projectilePool;

    private float _shootTimer;
    private Animator _animator;
    private Coroutine _resetAttackCoroutine; // Tham chiếu Coroutine để kiểm soát việc dừng/bắt đầu

    // THÊM: Tham chiếu đến AudioSource
    private AudioSource _audioSource;

    // HẰNG SỐ: Tên tham số Animator
    private const string ATTACK_PARAM = "tancong";
    // HẰNG SỐ: Độ trễ để reset animation (cần khớp với độ dài animation tấn công)
    private const float ATTACK_ANIM_DURATION = 0.1f;


    private void OnEnable()
    {
        Bongma.OnEnemyDestroyed += HandleEnemyDestroyed;
    }

    private void OnDisable()
    {
        Bongma.OnEnemyDestroyed -= HandleEnemyDestroyed;
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        // THÊM: Lấy component AudioSource
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        _circleCollider = GetComponent<CircleCollider2D>();
        if (_circleCollider != null)
        {
            _circleCollider.radius = data.range;
        }

        _projectilePool = GetComponent<ObjectPooler>();
        _shootTimer = data.shootInterval;

        // Đảm bảo tướng bắt đầu ở trạng thái đứng yên
        if (_animator != null)
        {
            _animator.SetBool(ATTACK_PARAM, false);
        }
    }

    private void Update()
    {
        // 1. Dọn dẹp danh sách quái
        _enemiesInRange.RemoveAll(enemy => enemy == null || !enemy.gameObject.activeInHierarchy);

        bool targetFound = _enemiesInRange.Count > 0;

        // 2. Cập nhật Animation DUNGYEN/TANCONG
        if (_animator != null)
        {
            if (targetFound)
            {
                // Logic được quản lý trong Shoot()
            }
            else
            {
                // KHÔNG CÓ MỤC TIÊU -> Chắc chắn phải ở trạng thái đứng yên
                if (_animator.GetBool(ATTACK_PARAM))
                {
                    _animator.SetBool(ATTACK_PARAM, false);
                }
                // Dừng Coroutine nếu đang chạy và không có mục tiêu
                if (_resetAttackCoroutine != null)
                {
                    StopCoroutine(_resetAttackCoroutine);
                    _resetAttackCoroutine = null;
                }
            }
        }

        // 3. Logic Tấn công (Chỉ chạy Timer khi có mục tiêu)
        if (targetFound)
        {
            _shootTimer -= Time.deltaTime;
            if (_shootTimer <= 0)
            {
                _shootTimer = data.shootInterval;
                Shoot();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bongma"))
        {
            Bongma enemy = collision.GetComponent<Bongma>();
            if (enemy != null && !_enemiesInRange.Contains(enemy))
            {
                _enemiesInRange.Add(enemy);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Bongma"))
        {
            Bongma enemy = collision.GetComponent<Bongma>();
            if (enemy != null && _enemiesInRange.Contains(enemy))
            {
                _enemiesInRange.Remove(enemy);
            }
        }
    }


    private void Shoot()
    {
        if (_enemiesInRange.Count > 0)
        {
            // 1. Kích hoạt Animation tấn công
            if (_animator != null)
            {
                // Đảm bảo Coroutine cũ dừng lại trước khi chạy Coroutine mới
                if (_resetAttackCoroutine != null)
                {
                    StopCoroutine(_resetAttackCoroutine);
                }

                // Kích hoạt animation tấn công
                _animator.SetBool(ATTACK_PARAM, true);

                // Bắt đầu Coroutine để đưa về trạng thái đứng yên
                _resetAttackCoroutine = StartCoroutine(ResetAttackAnimation(ATTACK_ANIM_DURATION));
            }

            // 2. KÍCH HOẠT ÂM THANH TẤN CÔNG 
            if (_audioSource != null && _audioSource.clip != null)
            {
                _audioSource.Play();
            }

            // 3. LOGIC BẮN ĐẠN
            GameObject projectile = _projectilePool.GetPooledObject();
            projectile.transform.position = transform.position;
            projectile.SetActive(true);

            // Xoay tướng về phía mục tiêu (Tùy chọn)
            Vector3 targetPosition = _enemiesInRange[0].transform.position;
            Vector2 _shootDirection = (targetPosition - transform.position).normalized;

            // ... (Bạn có thể thêm logic xoay nhân vật tại đây nếu cần)

            projectile.GetComponent<Projectile>().Shoot(data, _shootDirection);
        }
    }

    // Coroutine để đưa Animator về trạng thái đứng yên
    private IEnumerator ResetAttackAnimation(float delay)
    {
        // Chờ một khoảng thời gian ngắn (0.1s)
        yield return new WaitForSeconds(delay);

        // Đặt lại tham số để chuyển về trạng thái 'dungyen' ngay lập tức (vì Has Exit Time đã tắt)
        if (_enemiesInRange.Count > 0 && _animator != null)
        {
            _animator.SetBool(ATTACK_PARAM, false);
        }
        _resetAttackCoroutine = null;
    }

    private void HandleEnemyDestroyed(Bongma enemy)
    {
        _enemiesInRange.Remove(enemy);
    }

    // Giữ nguyên OnDrawGizmos()
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, data.range);
    }
}