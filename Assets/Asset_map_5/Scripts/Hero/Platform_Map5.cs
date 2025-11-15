using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Platform_Map5 : MonoBehaviour
{
    public static event Action<Platform_Map5> OnPlatformClicked;
    [SerializeField] private LayerMask platformLayerMask;
    public static bool towerPanelOpen { get; set; } = false;


    private void Update()
    {
        if (towerPanelOpen || Time.timeScale == 0f)
            return;
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            RaycastHit2D raycastHit = Physics2D.Raycast(worldPoint, Vector2.zero, Mathf.Infinity, platformLayerMask);

            if (raycastHit.collider != null)
            {
                Platform_Map5 platform = raycastHit.collider.GetComponent<Platform_Map5>();
                if (platform != null)
                {
                    OnPlatformClicked?.Invoke(platform);
                }
            }
        }
    }

    public void PlaceTower(HeroData_Map5 data)
    {
        Instantiate(data.prefab, transform.position, Quaternion.identity, transform);
    }


}
