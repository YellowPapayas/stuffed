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
    TokenBar tokenBar;

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
    int dodgeCap;
    public int currCrit;
    public List<StatModifier> statMods;
    public List<PassiveModifier> passiveMods;
    public List<TokenTuple> tokens;

    BoxCollider2D coll;
    List<Character> affectedText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = stats.maxHealth;
        energy = stats.maxEnergy;
        coll = GetComponent<BoxCollider2D>();
        
        bar = transform.Find("Canvas").Find("Backbar").Find("Healthbar").gameObject.GetComponent<Healthbar>();
        highlight = transform.Find("Canvas").Find("Highlight").gameObject;
        highlight.SetActive(false);
        target = transform.Find("Canvas").Find("Target").gameObject;
        target.SetActive(false);
        statusBar = transform.Find("Overlay").Find("Status Bar").gameObject.GetComponent<StatusBar>();
        tokenBar = transform.Find("Overlay").Find("Token Bar").gameObject.GetComponent<TokenBar>();
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
        tokens = new List<TokenTuple>();

        foreach(Ability ab in abilities)
        {
            ab.CompleteReset();
        }

        dodgeCap = bm.dodgeCap;
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

    public void AddToken(TokenObject tokenObject, Character applier)
    {
        TokenTuple tokenTuple = new TokenTuple(tokenObject.actionCondition, tokenObject.conditionAmount, tokenObject.effect, applier, tokenObject.affectActivator);
        tokens.Add(tokenTuple);

        tokenBar.AddToken(tokenObject);
    }

    public void AddStatus(StatModifier statMod)
    {
        statMods.Add(statMod);
        statusBar.UpdateView();
    }

    public void OnDebuffsHit(List<StatModifier> debuffs)
    {
        foreach (StatModifier statMod in debuffs)
        {
            AddStatus(statMod.DeepCopy());
        }
    }

    public int GetDodge()
    {
        int dodgeMod = passiveMods.Where(mod => mod.type == StatType.Dodge).Sum(mod => mod.amount);
        return currDodge + dodgeMod;
    }

    public bool AccuracyCheck(int acc)
    {
        if(acc > GetDodge())
        {
            return true;
        } else
        {
            currDodge -= acc;
            if(currDodge < 0)
            {
                currDodge = 0;
            }
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
        int passMod = 0;
        if (st != StatType.Dodge)
        {
            passMod = passiveMods.Where(mod => mod.type == st).Sum(mod => mod.amount);
        }
        return baseVal + mod + passMod;
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

    public string GetPassiveString(StatType st)
    {
        int modamount = passiveMods.Where(m => m.type == st).Sum(m => m.amount); ;
        if (modamount > 0)
        {
            return $" <color=#CCFF00>(+{modamount})</color>";
        }
        else if (modamount < 0)
        {
            return $" <color=#FFCC00>({modamount})</color>";
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
            if (statMods[i].removeThisRound)
            {
                statMods[i].rounds--;
                if (statMods[i].rounds <= 0)
                {
                    RemoveStatusIndex(i);
                }
            }
        }

        for (int j = tokens.Count - 1; j >= 0; j--)
        {
            TokenTuple t = tokens[j];
            t.remainingRounds--;
            if(t.remainingRounds <= 0)
            {
                RemoveToken(j);
            }
        }

        foreach(Ability ab in abilities)
        {
            ab.ShouldCooldown();
            ab.ResetUses();
        }

        energy = stats.maxEnergy;
    }

    public void OnGeneralTurnEnd()
    {
        if (!justDodged)
        {
            if (currDodge + GetStat(StatType.Dodge) < dodgeCap)
            {
                if (GetStat(StatType.Dodge) > 0)
                {
                    currDodge += GetStat(StatType.Dodge);
                }
            }
            else
            {
                currDodge = dodgeCap;
            }
        }

        for (int i = statMods.Count - 1; i >= 0; i--)
        {
            if (!statMods[i].removeThisRound)
            {
                statMods[i].removeThisRound = true;
            }
        }

        justDodged = false;
    }

    public void OnHit(int damage, Character attacker)
    {
        int armorDmg = calcArmorDamage(damage);
        Damage(armorDmg);

        UpdateTokens(ActionCondition.OnHit, attacker);
    }

    public void OnActivateToken(Character activator)
    {
        for (int i = 0; i < tokens.Count; i++)
        {
            TokenTuple tuple = tokens[i];
            if (tuple.actionCondition == ActionCondition.OnActivateToken && tuple.applicant == activator)
            {
                tuple.amount -= 1;
                if (tuple.amount <= 0)
                {
                    tuple.Execute(tuple.applicant, this, activator);
                    RemoveToken(i);
                    i--;
                }
            }
        }
    }

    void UpdateTokens(ActionCondition actCond, Character activator)
    {
        for(int i = 0; i < tokens.Count; i++)
        {
            TokenTuple tuple = tokens[i];
            if(tuple.actionCondition == actCond)
            {
                tuple.amount -= 1;
                if(tuple.amount <= 0)
                {
                    tuple.Execute(tuple.applicant, this, activator);
                    RemoveToken(i);
                    i--;
                }
            }
        }
    }

    void RemoveToken(int pos)
    {
        tokens.RemoveAt(pos);
        tokenBar.RemoveToken(pos);
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

    public void OnRemoveValue(List<ValueModifier> valMods)
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
    }

    public void OnAddValue(List<ValueModifier> valMods)
    {
        foreach (ValueModifier valMod in valMods)
        {
            switch (valMod.value)
            {
                case CurrentValue.Dodge:
                    currDodge += valMod.amount;
                    if (currDodge > dodgeCap)
                    {
                        currDodge = dodgeCap;
                    }
                    break;
                case CurrentValue.Energy:
                    energy += valMod.amount;
                    break;
                case CurrentValue.Crit:
                    currCrit += valMod.amount;
                    break;
            }
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

    public void OnOtherCharacterDeath(Character other)
    {
        for (int i = tokens.Count - 1; i >= 0; i--)
        {
            if (tokens[i].applicant == other)
            {
                RemoveToken(i);
            }
        }
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
            int cost = ab.critCost;
            if(cost > max)
            {
                max = cost;
            }
        }
        return max;
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
public class PassiveModifier
{
    public StatType type;
    public int amount;

    public PassiveModifier(StatType st, int am)
    {
        type = st; amount = am;
    }

    public PassiveModifier DeepCopy()
    {
        return new PassiveModifier(type, amount);
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

public class TokenTuple
{
    public ActionCondition actionCondition;
    public int amount;
    public AbilityEffect effect;

    public Character applicant;
    public bool affectActivator;
    public int remainingRounds = 2;

    public TokenTuple(ActionCondition ac, int amt, AbilityEffect ae, Character applier, bool aa)
    {
        actionCondition = ac; amount = amt; effect = ae;
        applicant = applier; affectActivator = aa;
    }

    public void Execute(Character user, Character target, Character activator)
    {
        List<AbilityAction> actions = new List<AbilityAction>();
        effect.AddEffect(actions);
        foreach (AbilityAction act in actions)
        {
            if (affectActivator)
            {
                act.Execute(user, activator, true);
            }
            else
            {
                act.Execute(user, target, true);
            }
        }
    }
}

public enum CurrentValue
{
    Dodge,
    Energy,
    Crit
}