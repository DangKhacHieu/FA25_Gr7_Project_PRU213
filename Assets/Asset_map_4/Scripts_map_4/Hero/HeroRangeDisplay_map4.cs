using UnityEngine;
using UnityEngine.EventSystems;

public class HeroRangeDisplay_map4 : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private HeroData heroData;
    [SerializeField] private GameObject rangeIndicatorPrefab;

    private GameObject indicatorInstance;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (indicatorInstance == null)
        {
            indicatorInstance = Instantiate(rangeIndicatorPrefab, transform.position, Quaternion.identity, transform);

            float diameter = heroData.attackRange * 2f;
            indicatorInstance.transform.localScale = new Vector3(diameter, diameter, 1f);

            SpriteRenderer sr = indicatorInstance.GetComponentInChildren<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = new Color(0f, 0.82f, 1f, 0.39f);
            }
        }
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        if (indicatorInstance != null)
        {
            Destroy(indicatorInstance);
            indicatorInstance = null; // Cẩn thận hơn
        }

    }
}