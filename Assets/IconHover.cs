using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class IconHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public StatType statType;
    GameObject hoverBG;
    TMP_Text hoverText;
    Character owner;
    public Sprite posIcon;
    public Sprite negIcon;

    int total = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hoverText = transform.Find("Info Text Background/Status Info Text").gameObject.GetComponent<TMP_Text>();
        hoverBG = transform.Find("Info Text Background").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateView()
    {
        total = 0;
        owner = transform.parent.parent.parent.gameObject.GetComponent<Character>();

        foreach (StatModifier sm in owner.statMods)
        {
            if (sm.type == statType)
            {
                total += sm.amount;
            }
        }
        Image display = GetComponent<Image>();
        display.sprite = (total > 0) ? posIcon : negIcon;
    }

    string GetStatsDisplay()
    {
        string output = "";
        foreach(StatModifier sm in owner.statMods)
        {
            if (sm.type == statType)
            {
                if(sm.amount > 0)
                {
                    output += "<color=green>+";
                } else
                {
                    output += "<color=red>";
                }
                output += sm.amount + $"</color> ({sm.rounds} rounds)\n";
            }
        }
        return output;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hoverText.text = GetStatsDisplay();
        hoverBG.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hoverBG.SetActive(false);
    }
}
