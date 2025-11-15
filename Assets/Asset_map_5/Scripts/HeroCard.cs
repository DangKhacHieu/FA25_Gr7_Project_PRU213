using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroCard : MonoBehaviour
{
    [SerializeField] private Image towerImage;
    [SerializeField] private TMP_Text costText;

    private HeroData_Map5 _towerData;
    public static event Action<HeroData_Map5> OnTowerSelected;

    public void Initialize(HeroData_Map5 data)
    {
        _towerData = data;
        towerImage.sprite = data.sprite;
        costText.text = data.cost.ToString();
    }

    public void PlaceTower()
    {
        OnTowerSelected?.Invoke(_towerData);
    }

}
