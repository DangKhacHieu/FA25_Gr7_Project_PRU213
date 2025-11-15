using UnityEngine;

public class VFXAutoDestroy : MonoBehaviour
{
    private ParticleSystem _particleSystem;

    void Start()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        if (_particleSystem == null)
        {
            // Nếu không có Particle System, hủy đối tượng sau 1 giây
            Destroy(gameObject, 1f);
            return;
        }

        // Hủy đối tượng sau khi hệ thống hạt phát xong.
        // Dùng Duration + Start Lifetime để đảm bảo hiệu ứng kết thúc hoàn toàn.
        float duration = _particleSystem.main.duration + _particleSystem.main.startLifetime.constantMax;

        Destroy(gameObject, duration);
    }
}
