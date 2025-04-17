using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class IconHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public StatType statType;
    GameObject hoverBG;
    TMP_Text hoverText;
    Character owner;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hoverText = transform.Find("Info Text Background/Status Info Text").gameObject.GetComponent<TMP_Text>();
        hoverBG = transform.Find("Info Text Background").gameObject;
        owner = transform.parent.parent.parent.gameObject.GetComponent<Character>();
    }

    // Update is called once per frame
    void Update()
    {
        
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
