using UnityEngine;

public class SplittingAbility : MonoBehaviour, IAbility
{
    // Lượng sát thương cố định bị hấp thụ mỗi lần đánh
    [SerializeField] private float damageAbsorption = 5f;

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
        // Đăng ký vào sự kiện KÍCH HOẠT TRƯỚC KHI NHẬN SÁT THƯƠNG
        enemy.OnBeforeTakeDamage += AbsorbDamage;
    }

    private void OnDestroy()
    {
        if (_enemy != null)
        {
            _enemy.OnBeforeTakeDamage -= AbsorbDamage;
        }
    }

    private void AbsorbDamage(Bongma enemy, float damage)
    {
        // --- Logic hấp thụ sát thương ---

        // 1. Tính toán lượng sát thương sau khi hấp thụ (không bao giờ âm)
        float finalDamage = Mathf.Max(0, damage - damageAbsorption);

        // 2. Tính toán lượng sát thương đã được hấp thụ (hoặc chặn)
        float absorbedAmount = damage - finalDamage;

        if (absorbedAmount > 0)
        {
            // WORKAROUND: Do TakeDamage đã trừ máu trong Bongma.cs, 
            // ta phải gọi TakeDamage với giá trị âm (hồi máu) để hủy bỏ phần sát thương đã hấp thụ.

            // Ví dụ: Nếu nhận 10 sát thương và hấp thụ 5:
            // - TakeDamage gốc đã trừ 10 máu.
            // - Ta gọi TakeDamage(-5) để hồi 5 máu. Kết quả net là trừ 5 máu.
            enemy.TakeDamage(-absorbedAmount);

             Debug.Log($"{enemy.gameObject.name} hấp thụ {absorbedAmount} sát thương!");
        }

        // Nếu finalDamage == 0 (toàn bộ sát thương bị hấp thụ), 
        // thì không cần làm gì thêm vì đã xử lý qua TakeDamage(-absorbedAmount).

    }
}