using UnityEngine;

public class HeroPlacementManager : MonoBehaviour
{
    public static HeroPlacementManager Instance;

    private bool isPlacingHero = false;
    private GameObject heroToPlace;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (isPlacingHero && heroToPlace != null)
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0;
            heroToPlace.transform.position = mouseWorld;
            heroToPlace.SetActive(true);

            if (Input.GetMouseButtonDown(0))
            {
                isPlacingHero = false;
                heroToPlace = null;
                Debug.Log("✅ Đã đặt tướng!");
            }
        }
    }

    public void StartPlacing(GameObject heroPrefab)
    {
        if (heroToPlace != null)
        {
            Destroy(heroToPlace);
        }

        heroToPlace = Instantiate(heroPrefab);
        heroToPlace.SetActive(false);
        isPlacingHero = true;

        Debug.Log("🧩 Bắt đầu đặt tướng bằng chuột");
    }
}
