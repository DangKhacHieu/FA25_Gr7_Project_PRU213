using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageReductionAbility : MonoBehaviour, IAbility
{
    // THÊM: Prefab hiệu ứng (ví dụ: một vòng tròn hoặc khí màu xanh)
    [SerializeField] private GameObject auraRingPrefab;

    // Thuộc tính kỹ năng
    [SerializeField][Range(0f, 1f)] private float defenseReduction = 0.15f; // Giảm 15% sát thương nhận vào
    [SerializeField] private float range = 4f;
    [SerializeField] private float checkInterval = 0.5f; // Tần suất kiểm tra phạm vi
    private GameObject _activeAuraRing;

    private Bongma _enemy;

    // Dictionary để quản lý các quái đã được áp dụng hiệu ứng giảm sát thương
    private Dictionary<Bongma, bool> _affectedEnemies = new Dictionary<Bongma, bool>();

    private void Awake()
    {
        _enemy = GetComponent<Bongma>();
        SetupAbility(_enemy);
    }

    public void SetupAbility(Bongma enemy)
    {
        // Không cần đăng ký event từ chính nó, nó sử dụng Coroutine lặp lại
    }

    private void OnEnable()
    {
        // Bắt đầu Coroutine kiểm tra phạm vi khi quái vật được kích hoạt
        StartCoroutine(AuraCheckLoop());

        // 🌟 HIỆU ỨNG BẮT ĐẦU 🌟
        if (auraRingPrefab != null)
        {
            _activeAuraRing = Instantiate(auraRingPrefab, transform); // Đính kèm vào quái vật
            _activeAuraRing.SetActive(true);
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        RemoveAllDamageHooks();

        // 🌟 HIỆU ỨNG KẾT THÚC 🌟
        if (_activeAuraRing != null)
        {
            Destroy(_activeAuraRing);
            _activeAuraRing = null;
        }
    }

    private void RemoveAllDamageHooks()
    {
        foreach (Bongma ally in _affectedEnemies.Keys)
        {
            if (ally != null)
            {
                // Hủy đăng ký sự kiện trước khi bị tắt
                ally.OnBeforeTakeDamage -= ReduceAllyDamage;
            }
        }
        _affectedEnemies.Clear();
    }


    private IEnumerator AuraCheckLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(checkInterval);
            CheckAndApplyAura();
        }
    }

    private void CheckAndApplyAura()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, range);
        HashSet<Bongma> targetsInCurrentRange = new HashSet<Bongma>();

        // 1. Áp dụng Aura cho quái vật mới trong phạm vi
        foreach (var hit in hitColliders)
        {
            Bongma ally = hit.GetComponent<Bongma>();

            // Là đồng minh (bao gồm chính nó) và đang hoạt động
            if (ally != null && ally.gameObject.activeInHierarchy)
            {
                targetsInCurrentRange.Add(ally);

                // Nếu chưa bị ảnh hưởng, đăng ký sự kiện giảm sát thương
                if (!_affectedEnemies.ContainsKey(ally))
                {
                    ally.OnBeforeTakeDamage += ReduceAllyDamage;
                    _affectedEnemies.Add(ally, true);
                    Debug.Log($"[DefenseAura] {ally.gameObject.name} BẮT ĐẦU nhận giảm {defenseReduction * 100}% sát thương.");
                }
            }
        }

        // 2. Hủy Aura cho quái vật vừa thoát khỏi phạm vi
        List<Bongma> enemiesToRestore = new List<Bongma>();
        foreach (Bongma ally in _affectedEnemies.Keys)
        {
            // Nếu quái vật đã bị ảnh hưởng nhưng không còn trong phạm vi hiện tại hoặc đã chết/tắt
            if (!targetsInCurrentRange.Contains(ally) || ally == null || !ally.gameObject.activeInHierarchy)
            {
                enemiesToRestore.Add(ally);
            }
        }

        // Dọn dẹp và hủy đăng ký
        foreach (Bongma ally in enemiesToRestore)
        {
            if (ally != null)
            {
                ally.OnBeforeTakeDamage -= ReduceAllyDamage;
                Debug.Log($"[DefenseAura] {ally.gameObject.name} KẾT THÚC nhận giảm sát thương.");
            }
            _affectedEnemies.Remove(ally);
        }
    }

    // HÀM CAN THIỆP SÁT THƯƠNG: Giảm sát thương nhận vào
    private void ReduceAllyDamage(Bongma enemy, float damage)
    {
        if (damage > 0)
        {
            float reduction = damage * defenseReduction;

            // WORKAROUND: Gọi TakeDamage với giá trị âm để hủy bỏ phần sát thương đã giảm
            enemy.TakeDamage(-reduction);

            // 🌟 THÊM HIỆU ỨNG CHỚP MẮT (nếu cần) 🌟
            // Ví dụ: Kích hoạt một hiệu ứng chặn đòn nhỏ trên quái vật đang được bảo vệ.
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}