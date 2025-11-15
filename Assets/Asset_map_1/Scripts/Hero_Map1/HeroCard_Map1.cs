using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroCard_Map1 : MonoBehaviour
{
    [SerializeField] private Image HeroImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text costText;

    private HeroData_Map1 _heroData;
    public static event Action<HeroData_Map1> OnTowerSelected;

    public void Initialize(HeroData_Map1 data)
    {
        _heroData = data;
        nameText .text = data.displayName;
        HeroImage.sprite = data.sprite;
        costText.text = data.cost.ToString();
    }
    public void PlaceHero()
    {
        OnTowerSelected?.Invoke(_heroData);
    }


}
