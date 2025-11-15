using UnityEngine;

public class EvasionAbility : MonoBehaviour, IAbility
{
    [SerializeField] private float evasionChance = 0.15f; // 15% né tránh

    private Bongma _enemy;

    private void Awake()
    {
        _enemy = GetComponent<Bongma>();
        if (_enemy != null)
        {
            SetupAbility(_enemy);
        }
    }

    public void SetupAbility(Bongma enemy)
    {
        enemy.OnBeforeTakeDamage += AttemptEvasion;
    }

    private void OnDestroy()
    {
        if (_enemy != null)
        {
            _enemy.OnBeforeTakeDamage -= AttemptEvasion;
        }
    }

    private void AttemptEvasion(Bongma enemy, float damage)
    {
        if (damage > 0 && Random.value < evasionChance)
        {
            // Ghi đè logic nhận sát thương bằng cách không gọi TakeDamage gốc
            // *Lưu ý:* Logic này yêu cầu TakeDamage gốc không tự động trừ máu. 
            // Hiện tại trong Bongma.cs, ta đang trừ máu. Để chặn damage, ta cần thay đổi TakeDamage.

            // Do TakeDamage đã trừ máu, ta buộc phải hồi lại máu nếu né thành công.
            // Đây là một workaround đơn giản nhất:
            enemy.TakeDamage(-damage); // Giả vờ hồi lại lượng máu đã bị trừ
            Debug.Log($"{enemy.gameObject.name} né tránh thành công sát thương {damage}!");
        }
    }
}
