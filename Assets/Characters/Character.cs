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
    public bool playerControlled;
    bool justDodged = false;
    bool isTurn = false;
    TurnTaker tt;

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
        statusBar = transform.Find("Overlay").Find("Status Bar").gameObject.GetComponent<StatusBar>();
        actionText = transform.Find("Overlay").Find("Action Text").gameObject.GetComponent<TemporaryText>();

        bm = GameObject.Find("BattleManager").GetComponent<BattleManager>();

        if (playerControlled)
        {
            tt = new PlayerTurn();
        } else
        {
            tt = new EnemyTurn();
        }

        statMods = new List<StatModifier>();
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

    public bool OnDebuffsHit(List<StatModifier> debuffs, int acc)
    {
        if (acc > currDodge)
        {
            foreach (StatModifier statMod in debuffs)
            {
                AddStatus(statMod.DeepCopy());
            }
            return true;
        }
        else
        {
            justDodged = true;
            currDodge -= acc;
            return false;
        }
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

    public string GetStatString(StatType st)
    {
        int statamount = GetStat(st);
        if (statamount > 0)
        {
            return $" <color=green>(+{statamount})</color>";
        }
        else if (statamount < 0)
        {
            return $" <color=red>({statamount})</color>";
        }
        else
        {
            return "";
        }
    }

    public int GetModAmount(StatType st)
    {
        int mod = statMods.Where(m => m.type == st).Sum(m => m.amount);
        return mod;
    }

    public string GetModString(StatType st)
    {
        int modamount = GetModAmount(st);
        if (modamount > 0)
        {
            return $" <color=green>(+{modamount})</color>";
        }
        else if (modamount < 0)
        {
            return $" <color=red>({modamount})</color>";
        }
        else
        {
            return "";
        }
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
        } else if (energy < 0)
        {
            energy = 0;
        }

        currCrit += stats.crit;
        if (currCrit > stats.crit * 10)
        {
            currCrit = stats.crit * 10;
        }

        tt.TakeTurn(this);
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

    public int GetCurrentValue(CurrentValue cv)
    {
        switch (cv)
        {
            case CurrentValue.Dodge:
                return currDodge;
            case CurrentValue.Energy:
                return stats.maxEnergy + energy;
            case CurrentValue.Crit:
                return stats.crit + currCrit;
            default:
                return 0;
        }
    }

    public bool OnRemoveValue(List<ValueModifier> valMods, int acc)
    {
        if (acc > currDodge)
        {
            foreach (ValueModifier valMod in valMods)
            {
                switch (valMod.value)
                {
                    case CurrentValue.Dodge:
                        currDodge -= valMod.amount;
                        if (currDodge < 0)
                        {
                            currDodge = 0;
                        }
                        break;
                    case CurrentValue.Energy:
                        energy -= valMod.amount;
                        break;
                    case CurrentValue.Crit:
                        currCrit -= valMod.amount;
                        if (currCrit < 0)
                        {
                            currCrit = 0;
                        }
                        break;
                }
            }
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

    public void OnDeath()
    {
        bm.OnCharacterDeath(this);

        Destroy(this.gameObject);
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

    public int HighestCritCost()
    {
        int max = 0;
        foreach(Ability ab in abilities)
        {
            int cost = ab.critEffect.critCost;
            if(cost > max)
            {
                max = cost;
            }
        }
        return max;
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
        StatsDisplay sd = (StatsDisplay) FindAnyObjectByType(typeof(StatsDisplay), FindObjectsInactive.Include);
        sd.gameObject.SetActive(true);
        sd.DisplayStats(this);
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

[System.Serializable]
public class ValueModifier
{
    public CurrentValue value;
    public int amount;

    public ValueModifier(CurrentValue cv, int am)
    {
        value = cv; amount = am;
    }

    public ValueModifier DeepCopy()
    {
        return new ValueModifier(value, amount);
    }
}

public enum CurrentValue
{
    Dodge,
    Energy,
    Crit
}