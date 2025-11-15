using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroCard_map_4 : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private Button button;

    private HeroData _data;
    public static System.Action<HeroData> OnHeroSelected;

    private void Awake()
    {
        if (!button) button = GetComponent<Button>();
        if (button)
        {
            button.onClick.RemoveListener(PlaceHero);
            button.onClick.AddListener(PlaceHero);
        }
    }

    public void Initialize(HeroData data)
    {
        _data = data;
        if (nameText) nameText.text = data.displayName;
        if (costText) costText.text = data.cost.ToString();
        if (iconImage) iconImage.sprite = data.icon;
    }

    public void PlaceHero()
    {
        if (_data == null) return;
        OnHeroSelected?.Invoke(_data);
    }
}
