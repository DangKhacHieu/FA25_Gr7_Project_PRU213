// Assets/Scripts/Platform.cs
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Platform : MonoBehaviour
{
    public static event Action<Platform> OnPlatformClicked;
    [SerializeField] private LayerMask platformLayerMask;
    public static bool towerPanelOpen { get; set; } = false; // dùng chung cho panel unit

    private void Update()
    {
        if (towerPanelOpen || Time.timeScale == 0f) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero, Mathf.Infinity, platformLayerMask);
            if (hit.collider != null)
            {
                Platform p = hit.collider.GetComponent<Platform>();
                if (p != null) OnPlatformClicked?.Invoke(p);
            }
        }
    }

    public void PlaceTower(TowerData data)
    {
        Instantiate(data.prefab, transform.position, Quaternion.identity, transform);
    }

    public void PlaceHero(HeroData data)
    { 
        Instantiate(data.prefab, transform.position, Quaternion.identity, transform);
    }
}
