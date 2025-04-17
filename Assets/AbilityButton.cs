using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityButton : MonoBehaviour
{
    Ability ability;
    TMP_Text text;
    Character user;
    Image icon;

    BattleManager bm;

    void Start()
    {
        bm = GameObject.Find("BattleManager").GetComponent<BattleManager>();
    }

    public void Setup(Ability ab, Character ch)
    {
        ability = ab; 
        user = ch;
        text = transform.Find("Ability Name").gameObject.GetComponent<TMP_Text>();
        text.text = ab.abilityName;
        icon = GetComponent<Image>();
        icon.sprite = ab.abilityImage;
    }

    public void OnClick()
    {
        bm.StartTargeting(ability);
    }
}
