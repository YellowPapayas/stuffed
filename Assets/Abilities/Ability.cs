using UnityEngine;
using System.Collections.Generic;

public abstract class Ability : ScriptableObject
{
    public string abilityName;
    public Sprite abilityImage;

    protected string abilityDescription;

    public TargetType targetType;
    public int energyCost;
    public int critCost;

    public int cooldown = 0;
    int currCooldown = 0;
    bool putOnCooldown = false;

    // <=0 means it can be used as many times as possible per turn
    public int usesPerTurn = 0;
    int currUses = 0;

    public AbilityEffect critEffect;
    public List<AbilityAction> actions;
    public List<ConditionalTuple> conditions;

    public abstract void AddActions();

    public virtual void Activate(Character user, List<Character> targets, bool isCrit)
    {
        bool selfTargetted = false;
        foreach (Character target in targets)
        {
            SetupActions(user, target, isCrit);

            for (int i = 0; i < actions.Count; i++)
            {
                AbilityAction act = actions[i];
                if (act.targetSelf)
                {
                    if (!selfTargetted)
                    {
                        act.Execute(user, user, true);
                    }
                }
                else
                {
                    act.Execute(user, target, DoesHit(user, target, isCrit));
                }
            }

            selfTargetted = true;
        }

        putOnCooldown = true;
        currUses += 1;
    }

    public virtual bool DoesHit(Character user, Character target, bool isCrit)
    {
        return true;
    }

    public virtual void SetupActions(Character user, Character target, bool isCrit)
    {
        AddActions();

        foreach(ConditionalTuple ct in conditions) {
            if(ct.cond.CheckCondition(this, user, target, isCrit))
            {
                ct.effect.AddEffect(actions);
            }
        }

        if (isCrit)
        {
            critEffect.AddEffect(actions);
        }
    }

    public virtual string FormatDescription(Character user)
    {
        return "";
    }

    public string FullDescription(Character user)
    {
        string conds = "";
        for(int i = 0; i < conditions.Count; i++)
        {
            ConditionalTuple ct = conditions[i];
            conds += ct.cond.ConditionString();
            conds += ct.effect.AddDescription() + (i < conditions.Count-1 ? "\n" : "");
        }
        string important = "";
        if (usesPerTurn > 0)
        {
            important += $"Uses per Turn: {usesPerTurn}";
        }
        if (usesPerTurn > 0 && cooldown > 0) {
            important += " / ";
        }
        if (cooldown > 0) {
            important += $"Cooldown: {cooldown}";
        }
        return $"<color=#FFBB00>{energyCost} Energy</color> " + important + FormatDescription(user) + (conditions.Count > 0 ? "\n" + conds : "");
    }

    public string CritDescription(Character user)
    {
        return FullDescription(user) + $"\n<color=#66CCCC>{critCost} CRIT</color>\n<color=#66AADD>" + critEffect.AddDescription() + "</color>";
    }

    public void ActionText(Character user, Character target, bool isCrit)
    {
        AddActions();

        foreach (ConditionalTuple ct in conditions)
        {
            if (ct.cond.CheckCondition(this, user, target, isCrit))
            {
                ct.effect.AddEffect(actions);
            }
        }

        if (isCrit)
        {
            critEffect.AddEffect(actions);
        }

        string selfText = "";
        string actText = "";
        for (int i = 0; i < actions.Count; i++)
        {
            AbilityAction act = actions[i];
            if (act.targetSelf)
            {
                if (selfText.Length > 0)
                {
                    selfText += "\n";
                }
                selfText += act.GetActionText(user, user, true);
            }
            else
            {
                if (actText.Length > 0)
                {
                    actText += "\n";
                }
                actText += act.GetActionText(user, target, DoesHit(user, target, isCrit));
            }
        }
        if (!DoesHit(user, target, isCrit))
        {
            actText = "<color=green>DODGED</color>\n";
        }
        user.gameObject.GetComponent<CharacterUI>().DisplayActionPerm(selfText);
        target.gameObject.GetComponent<CharacterUI>().DisplayActionPerm(actText);
    }

    public virtual (float, float) CalcActionValue(Character user, Character target, ActionValues mults)
    {
        AddActions();
        List<AbilityAction> condCritActions = new List<AbilityAction>();

        foreach (ConditionalTuple ct in conditions)
        {
            if (ct.cond.CheckCondition(this, user, target, false))
            {
                ct.effect.AddEffect(actions);
            }
            else if (ct.cond.CheckCondition(this, user, target, true))
            {
                ct.effect.AddEffect(condCritActions);
            }
        }

        float total = 0f;
        foreach (AbilityAction act in actions)
        {
            total += act.GetActionValue(user, target, DoesHit(user, target, false), mults);
        }

        critEffect.AddEffect(actions);

        float critTotal = 0f;
        foreach (AbilityAction act in actions)
        {
            critTotal += act.GetActionValue(user, target, DoesHit(user, target, true), mults);
        }
        foreach(AbilityAction act in condCritActions)
        {
            critTotal += act.GetActionValue(user, target, DoesHit(user, target, true), mults);
        }

        return (total, critTotal);
    }

    public bool Equals(Ability other)
    {
        return other != null && abilityName == other.abilityName;
    }

    public string StringTargetType()
    {
        switch (targetType)
        {
            case TargetType.SingleEnemy:
                return "a single enemy";
            case TargetType.SingleAlly:
                return "a single ally";
            case TargetType.MultiEnemy:
                return "a line of enemies";
            case TargetType.MultiAlly:
                return "a line of allies";
            case TargetType.EnemyParty:
                return "the enemy party";
            case TargetType.AllyParty:
                return "the ally party";
            default:
                return "";
        }
    }

    public void ShouldCooldown()
    {
        if (putOnCooldown)
        {
            currCooldown = cooldown;
            putOnCooldown = false;
        } else if(IsOnCooldown())
        {
            currCooldown -= 1;
        }
    }

    public void ResetUses()
    {
        currUses = 0;
    }

    public bool IsOnCooldown()
    {
        return currCooldown > 0;
    }

    public int GetCooldown()
    {
        return currCooldown;
    }

    public bool CanUse()
    {
        return !IsOnCooldown() && (usesPerTurn <= 0 || currUses < usesPerTurn);
    }

    public void CompleteReset()
    {
        currCooldown = 0;
        ResetUses();
    }
}

[System.Serializable]
public class ConditionalTuple
{
    public Conditional cond;
    public AbilityEffect effect;
}

public enum TargetType
{
    SingleEnemy,
    SingleAlly,
    MultiEnemy,
    MultiAlly,
    EnemyParty,
    AllyParty
}