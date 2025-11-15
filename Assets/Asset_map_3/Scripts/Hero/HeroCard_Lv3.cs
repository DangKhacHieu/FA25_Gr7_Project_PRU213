using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroCard_Lv3 : MonoBehaviour
{
    [SerializeField] private Image heroImage;
    [SerializeField] private TMP_Text costText;

    private HeroData_Lv3 _heroData;
    public static event Action<HeroData_Lv3> OnHeroSelected;

    public void Initialize(HeroData_Lv3 data)
    {
        _heroData = data;
        heroImage.sprite = data.sprite;
        costText.text = data.cost.ToString();
    }

    public void PlaceTower()
    {
        OnHeroSelected?.Invoke(_heroData);
    }

}
