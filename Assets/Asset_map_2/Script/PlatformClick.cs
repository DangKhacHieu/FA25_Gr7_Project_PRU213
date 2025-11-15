using UnityEngine;

public class PlatformClick : MonoBehaviour
{
    [HideInInspector] public GameObject placedHero; // ✅ tướng đang có trên platform

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null && hit.collider.GetComponent<PlatformClick>() != null)
            {
                PlatformClick platform = hit.collider.GetComponent<PlatformClick>();
                Debug.Log("🟩 Clicked on platform: " + platform.name);

                if (HeroSelectionUI.Instance == null)
                {
                    HeroSelectionUI ui = FindObjectOfType<HeroSelectionUI>(true);
                    if (ui != null)
                    {
                        ui.gameObject.SetActive(true);
                        HeroSelectionUI.Instance = ui;
                    }
                    else
                    {
                        Debug.LogWarning("⚠️ Không tìm thấy HeroSelectionUI trong scene!");
                        return;
                    }
                }

                // ✅ Nếu chưa có tướng thì mở UI mua
                if (platform.placedHero == null)
                    HeroSelectionUI.Instance.ShowAtPlatform(platform);
                else
                    HeroSelectionUI.Instance.ShowRemoveUI(platform);
            }
        }
    }
}
