// Assets/Scripts/Platform_map_4.cs
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Platform_map_4 : MonoBehaviour
{
    public static event Action<Platform_map_4> OnPlatformClicked;
    [SerializeField] private LayerMask platformLayerMask;
    public static bool towerPanelOpen { get; set; } = false;

    [Header("Fire Logic")]
    [Tooltip("Prefab hiệu ứng lửa sẽ bật khi bị boss đốt")]
    [SerializeField] private GameObject fireEffectPrefab;
    [Tooltip("Số lần click để dập lửa")]
    [SerializeField] private int clicksToExtinguish = 5;

    private bool _isOnFire = false;
    private GameObject _currentFireEffect;
    private int _currentFireClicks = 0;
    private Coroutine _burnCoroutine;

    private void Update()
    {
        // Chỉ xử lý click chuột trái
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;

        // Dùng Raycast để xem có click trúng platform nào không
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero, Mathf.Infinity, platformLayerMask);

        // Nếu không click trúng gì, hoặc không trúng platform này -> bỏ qua
        if (hit.collider == null || hit.collider.gameObject != this.gameObject)
        {
            return;
        }

        // === Đã click trúng Platform NÀY ===

        // 1. Ưu tiên: Nếu đang cháy -> xử lý dập lửa
        if (_isOnFire)
        {
            HandleFireClick();
        }
        // 2. Nếu không cháy: Mở panel xây hero (logic cũ)
        else if (!towerPanelOpen && Time.timeScale != 0f)
        {
            OnPlatformClicked?.Invoke(this);
        }
    }

    /// <summary>
    /// Xử lý khi người chơi click vào platform đang cháy
    /// </summary>
    private void HandleFireClick()
    {
        _currentFireClicks++;
        Debug.Log($"Click dập lửa: {_currentFireClicks} / {clicksToExtinguish}");

        // (Bạn có thể thêm 1 âm thanh "click" hoặc hiệu ứng nhỏ ở đây)

        if (_currentFireClicks >= clicksToExtinguish)
        {
            ExtinguishFire();
        }
    }

    /// <summary>
    /// Đặt Hero (logic cũ, thêm kiểm tra đang cháy)
    /// </summary>
    public void PlaceHero(HeroData data)
    {
        if (_isOnFire)
        {
            Debug.Log("Không thể đặt hero, platform đang cháy!");
            return;
        }
        Instantiate(data.prefab, transform.position, Quaternion.identity, transform);
    }

    // ===== PHƯƠNG THỨC MỚI MÀ BOSS SẼ GỌI =====

    /// <summary>
    /// Kích hoạt trạng thái "cháy" trên platform này.
    /// Bắt đầu đếm ngược 5 giây để phá hủy hero.
    /// </summary>
    /// <param name="duration">Thời gian (giây) trước khi hero bị hủy</param>
    public void SetOnFire(float duration)
    {
        if (_isOnFire) return; // Không đốt nữa nếu đang cháy

        _burnCoroutine = StartCoroutine(BurnRoutine(duration));
    }

    private IEnumerator BurnRoutine(float duration)
    {
        _isOnFire = true;
        _currentFireClicks = 0; // Reset bộ đếm click

        // Bật hiệu ứng lửa
        if (fireEffectPrefab != null)
        {
            _currentFireEffect = Instantiate(fireEffectPrefab, transform.position, Quaternion.identity, transform);
            _currentFireEffect.SetActive(true);
        }

        // Đợi hết thời gian
        yield return new WaitForSeconds(duration);

        // === HẾT GIỜ ===
        // Nếu coroutine chạy đến đây, nghĩa là _isOnFire VẪN = true
        // (Vì nếu đã dập lửa, coroutine đã bị Stop)

        Debug.Log("HẾT GIỜ! Hero đã bị thiêu rụi!");

        // Hậu quả: Hủy Hero
        DestroyHeroOnPlatform();

        // Tự dập lửa (vì đã cháy xong)
        _isOnFire = false;
        if (_currentFireEffect != null)
        {
            Destroy(_currentFireEffect);
        }
    }

    /// <summary>
    /// Người chơi đã click đủ 5 lần -> Dập lửa thành công
    /// </summary>
    private void ExtinguishFire()
    {
        if (!_isOnFire) return;

        Debug.Log("Đã dập lửa thành công!");
        _isOnFire = false;

        // Dừng coroutine đếm ngược
        if (_burnCoroutine != null)
        {
            StopCoroutine(_burnCoroutine);
        }

        // Tắt hiệu ứng lửa
        if (_currentFireEffect != null)
        {
            Destroy(_currentFireEffect);
        }
        _currentFireClicks = 0;
    }

    /// <summary>
    /// Tìm và phá hủy bất kỳ child nào có script Hero (hoặc Tower)
    /// </summary>
    private void DestroyHeroOnPlatform()
    {
        var heroScript = GetComponentInChildren<HeroAttack_map_4>();

        if (heroScript != null)
        {
            Transform heroRoot = heroScript.transform;

            while (heroRoot.parent != null && heroRoot.parent != this.transform)
            {
                heroRoot = heroRoot.parent;
            }

            if (heroRoot.parent == this.transform)
            {
                Debug.Log($"Phá hủy gốc hero: {heroRoot.name}");
                Destroy(heroRoot.gameObject);
            }
        }
    }
}