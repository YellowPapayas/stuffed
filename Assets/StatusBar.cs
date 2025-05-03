using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class StatusBar : MonoBehaviour
{
    Character owner;
    GameObject armorIcon;
    GameObject attackIcon;
    GameObject accuracyIcon;
    GameObject dodgeIcon;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        owner = transform.parent.parent.GetComponent<Character>();

        attackIcon = transform.Find("Attack Icon").gameObject;
        armorIcon = transform.Find("Armor Icon").gameObject;
        accuracyIcon = transform.Find("Accuracy Icon").gameObject;
        dodgeIcon = transform.Find("Dodge Icon").gameObject;

        ClearView();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    GameObject statTypeToIcon(StatType st)
    {
        switch (st)
        {
            case StatType.Armor:
                return armorIcon;
            case StatType.Dodge:
                return dodgeIcon;
            case StatType.Attack:
                return attackIcon;
            case StatType.Accuracy:
                return accuracyIcon;
        }
        return null;
    }

    void ClearView()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    public void UpdateView()
    {
        ClearView();

        foreach (StatModifier sm in owner.statMods)
        {
            GameObject icon = statTypeToIcon(sm.type);
            if (icon != null)
            {
                icon.SetActive(true);
                icon.GetComponent<IconHover>().UpdateView();
            }
        }
    }
}
