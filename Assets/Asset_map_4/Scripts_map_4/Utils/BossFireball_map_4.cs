using UnityEngine;

public class BossFireball_map_4 : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float burnDuration = 5f;
    [SerializeField] private float explodeRadius = 0.5f; // bán kính tìm platform nếu fallback

    private Vector3 _targetPosition;
    private bool _isLaunched = false;
    private Platform_map_4 _targetPlatformRef = null; // nếu boss truyền platform trực tiếp

    /// <summary>
    /// Launch tới vị trí, có thể kèm Platform reference để không phải dò bằng physics khi nổ.
    /// </summary>
    public void Launch(Vector3 targetPosition, Platform_map_4 platform = null)
    {
        _targetPosition = targetPosition;
        _targetPlatformRef = platform;
        _isLaunched = true;

        gameObject.SetActive(true);

        Vector2 dir = (targetPosition - transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
        Debug.Log($"[Fireball] Launch → {targetPosition} (platformRef={(platform != null ? platform.name : "null")})");
    }

    private void Update()
    {
        if (!_isLaunched) return;

        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, step);

        if (Vector3.Distance(transform.position, _targetPosition) < 0.15f)
        {
            Explode();
        }
    }

    private void Explode()
    {
        _isLaunched = false;
        Debug.Log("[Fireball] 💥 Nổ!");

        // Nếu có tham chiếu platform được boss truyền thì dùng luôn
        if (_targetPlatformRef != null)
        {
            _targetPlatformRef.SetOnFire(burnDuration);
            Debug.Log($"[Fireball] Đốt platform (direct): {_targetPlatformRef.name}");
        }
        else
        {
            // fallback: tìm platform gần vị trí nổ
            var platform = FindPlatformNear(_targetPosition);
            if (platform != null)
            {
                platform.SetOnFire(burnDuration);
                Debug.Log($"[Fireball] Đốt platform (found): {platform.name}");
            }
            else
            {
                Debug.LogWarning("[Fireball] Nổ nhưng không tìm thấy platform gần vị trí nổ!");
            }
        }

        Destroy(gameObject);
    }

    // fallback: tìm platform bằng OverlapCircleAll, không dùng LayerMask.GetMask cứng
    private Platform_map_4 FindPlatformNear(Vector3 position)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(position, explodeRadius);
        foreach (var hit in hits)
        {
            var p = hit.GetComponent<Platform_map_4>();
            if (p != null)
                return p;
        }
        return null;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_targetPosition, explodeRadius);
    }
#endif
}
