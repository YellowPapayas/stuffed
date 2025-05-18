using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityButton : MonoBehaviour
{
    Ability ability;
    TMP_Text text;
    TMP_Text cooldown;
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
        cooldown = transform.Find("Cooldown Text").gameObject.GetComponent<TMP_Text>();
        icon = GetComponent<Image>();
        icon.sprite = ab.abilityImage;

        UpdateView();
    }

    public void UpdateView()
    {
        Button button = gameObject.GetComponent<Button>();

        if (ability.CanUse())
        {
            button.interactable = true;
        } else
        {
            button.interactable = false;
        }

        if (ability.IsOnCooldown())
        {
            cooldown.text = "" + ability.GetCooldown();
        }
    }

    public void OnClick()
    {
        bm.StartTargeting(ability);
    }
}
