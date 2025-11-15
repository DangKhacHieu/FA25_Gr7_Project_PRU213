using UnityEngine;
using System.Collections;

public class RageAbility : MonoBehaviour, IAbility
{
    [SerializeField] private float speedMultiplier = 1.5f;
    [SerializeField] private float duration = 5f;

    private Bongma _enemy;
    private float _originalSpeed;

    private void Awake()
    {
        _enemy = GetComponent<Bongma>();
        if (_enemy != null)
        {
            // Lấy tốc độ gốc ban đầu (từ Scriptable Object)
            _originalSpeed = _enemy.Data.speed;
            SetupAbility(_enemy);
        }
    }

    public void SetupAbility(Bongma enemy)
    {
        enemy.OnHealthLow += ActivateRage;
    }

    private void OnDestroy()
    {
        // Kiểm tra an toàn trước khi hủy đăng ký
        if (this != null && _enemy != null)
        {
            _enemy.OnHealthLow -= ActivateRage;
        }
    }

    private void ActivateRage(Bongma enemy)
    {
        if (enabled)
        {
            Debug.Log($"{enemy.gameObject.name} kích hoạt Rage!");
            StartCoroutine(RageCoroutine(enemy));
            enabled = false;
        }
    }

    private IEnumerator RageCoroutine(Bongma enemy)
    {
        // Kiểm tra an toàn trước khi thực hiện
        if (enemy == null || !enemy.gameObject.activeInHierarchy) yield break;

        // Tăng tốc độ bằng SetSpeed mới (sửa đổi _currentSpeed)
        enemy.SetSpeed(_originalSpeed * speedMultiplier);

        // Chờ thời gian Rage
        yield return new WaitForSeconds(duration);

        // Kiểm tra an toàn trước khi khôi phục
        if (enemy != null && enemy.gameObject.activeInHierarchy)
        {
            // Đặt lại tốc độ về _originalSpeed (giá trị đã đọc từ đầu)
            enemy.SetSpeed(_originalSpeed);
        }
    }
}