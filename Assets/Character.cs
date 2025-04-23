using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class Character : MonoBehaviour, IClickable
{
    Healthbar bar;
    GameObject highlight;
    GameObject target;
    TemporaryText actionText;
    StatusBar statusBar;

    Color highlightColor = new Color(228 / 255f, 1f, 0f, 120 / 255f);
    Color turnColor = new Color(120 / 255f, 180 / 255f, 120 / 255f, 90 / 255f);

    BattleManager bm;

    public CharacterStats stats;
    public List<Ability> abilities;
    public bool teamSide;
    bool justDodged = false;
    bool isTurn = false;

    public int health;
    public int energy;
    public int currDodge;
    public int currCrit;
    public List<StatModifier> statMods;

    BoxCollider2D coll;
    List<Character> affectedText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = stats.maxHealth;
        coll = GetComponent<BoxCollider2D>();
        
        bar = transform.Find("Canvas").Find("Backbar").Find("Healthbar").gameObject.GetComponent<Healthbar>();
        highlight = transform.Find("Canvas").Find("Highlight").gameObject;
        highlight.SetActive(false);
        target = transform.Find("Canvas").Find("Target").gameObject;
        target.SetActive(false);
        statusBar = transform.Find("Canvas").Find("Status Bar").gameObject.GetComponent<StatusBar>();
        actionText = transform.Find("Overlay").Find("Action Text").gameObject.GetComponent<TemporaryText>();

        bm = GameObject.Find("BattleManager").GetComponent<BattleManager>();

        statMods = new List<StatModifier>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetHighlight(bool show)
    {
        Image light = highlight.GetComponent<Image>();
        if (isTurn)
        {
            highlight.SetActive(true);
            if (show)
            {
                light.color = highlightColor;
            }
            else
            {
                light.color = turnColor;
            }
        }
        else
        {
            light.color = highlightColor;
            highlight.SetActive(show);
        }
    }

    public void AddStatus(StatModifier statMod)
    {
        statMods.Add(statMod);
        statusBar.UpdateView();
    }

    public void RemoveStatusIndex(int index)
    {
        statMods.RemoveAt(index);
        statusBar.UpdateView();
    }

    public int GetStat(StatType st)
    {
        int baseVal = stats.GetStat(st);
        int mod = statMods.Where(m => m.type == st).Sum(m => m.amount);
        return baseVal + mod;
    }

    public void OnTurnStart()
    {
        isTurn = true;
        SetHighlight(false);

        for (int i = statMods.Count - 1; i >= 0; i--)
        {
            if (statMods[i].amount >= 0)
            {
                statMods[i].rounds--;
                if (statMods[i].rounds <= 0)
                {
                    RemoveStatusIndex(i);
                }
            }
        }

        energy += stats.maxEnergy;
        if (energy > stats.maxEnergy)
        {
            energy = stats.maxEnergy;
        }

        currCrit += stats.crit;
        if (currCrit > stats.crit * 10)
        {
            currCrit = stats.crit * 10;
        }
    }

    public void OnTurnEnd()
    {
        isTurn = false;
        SetHighlight(false);

        for (int i = statMods.Count - 1; i >= 0; i--)
        {
            if (statMods[i].amount < 0)
            {
                statMods[i].rounds--;
                if (statMods[i].rounds <= 0)
                {
                    RemoveStatusIndex(i);
                }
            }
        }
    }

    public void OnGeneralTurnEnd()
    {
        if (!justDodged)
        {
            if (currDodge < stats.dodge * 10)
            {
                currDodge += stats.dodge;
            }
            else
            {
                currDodge = stats.dodge * 10;
            }
        }

        justDodged = false;
    }

    public bool OnHit(int damage, int acc)
    {
        int armorDmg = calcArmorDamage(damage);
        if (acc > currDodge)
        {
            Damage(armorDmg);
            return true;
        } else
        {
            justDodged = true;
            currDodge -= acc;
            return false;
        }
    }

    public void OnHeal(int amount)
    {
        Heal(amount);
    }

    float ArmorReduction()
    {
        return 100f / (GetStat(StatType.Armor) + 100);
    }

    public int calcArmorDamage(int damage)
    {
        return Mathf.FloorToInt(damage * ArmorReduction());
    }

    void Damage(int damage)
    {
        if (health < damage)
        {
            health = 0;
        } else
        {
            health -= damage;
        }
        bar.UpdateView();
    }

    void Heal(int amount)
    {
        if (health + amount > stats.maxHealth)
        {
            health = stats.maxHealth;
        } else
        {
            health += amount;
        }
        bar.UpdateView();
    }

    public void DisplayAbilityAction(string action)
    {
        actionText.DisplayText(action, 300);
    }

    public void DisplayActionPerm(string perm)
    {
        actionText.PermText(perm);
    }

    public void ActionAdd(string add)
    {
        actionText.AddText(add);
    }

    public void ActionDuration(int duration)
    {
        actionText.SetDuration(duration);
    }

    public void ActionOff()
    {
        actionText.PermOff();
    }

    public void OnLeftClick()
    {
        // do nothing
    }

    public void OnRightClick()
    {
        // do nothing
    }

    public void OnHover()
    {
        target.SetActive(true);
        affectedText = bm.PreviewAbility(this);
    }

    public void OffHover()
    {
        target.SetActive(false);
        if (affectedText != null)
        {
            foreach (Character ch in affectedText)
            {
                ch.ActionOff();
            }
        }
    }
}
