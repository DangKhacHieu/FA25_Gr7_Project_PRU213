using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Platform_Map1 : MonoBehaviour
{
    public static event Action<Platform_Map1> OnPlatformClicked;

    [SerializeField] private LayerMask platformLayerMask;

    public static bool heroPanelOpen { get; set; } = false;

    private Hero_Map1 _currentHero; // 🟢 Hero hiện tại trên platform
    private float _lastClickTime;   // ⏱️ Lưu thời gian click trước
    private const float DoubleClickThreshold = 0.3f; // Giới hạn double click

    private void Update()
    {
        if (heroPanelOpen)
            return;

        // 🖱️ Phát hiện click chuột trái
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            RaycastHit2D raycastHit = Physics2D.Raycast(worldPoint, Vector2.zero, Mathf.Infinity, platformLayerMask);

            if (raycastHit.collider != null)
            {
                Platform_Map1 platform = raycastHit.collider.GetComponent<Platform_Map1>();
                if (platform != null)
                {
                    float timeSinceLastClick = Time.time - _lastClickTime;

                    // 🔹 Nếu click đúp
                    if (timeSinceLastClick <= DoubleClickThreshold)
                    {
                        platform.HandleDoubleClick();
                    }
                    else
                    {
                        platform.HandleSingleClick();
                    }

                    _lastClickTime = Time.time;
                }
            }
        }
    }

    private void HandleSingleClick()
    {
        // ✅ Nếu platform chưa có hero → mở bảng chọn
        if (_currentHero == null)
        {
            OnPlatformClicked?.Invoke(this);
        }
        // ❌ Nếu đã có hero → không làm gì
    }

    private void HandleDoubleClick()
    {
        // 🗑️ Nếu đã có hero → xoá
        if (_currentHero != null)
        {
            RemoveHero();
        }
    }

    public void PlaceHero(HeroData_Map1 data)
    {
        if (_currentHero != null)
        {
            Debug.Log("Platform đã có hero!");
            return;
        }

        GameObject heroObj = Instantiate(data.prefab, transform.position, Quaternion.identity, transform);
        _currentHero = heroObj.GetComponent<Hero_Map1>();
    }

    private void RemoveHero()
    {
        if (_currentHero != null)
        {
            Destroy(_currentHero.gameObject);
            _currentHero = null;
            Debug.Log("🗑️ Hero đã bị xoá khỏi platform bằng double click.");
        }
    }
}
