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

    public AbilityEffect critEffect;
    public List<AbilityAction> actions;
    public List<ConditionalTuple> conditions;

    public abstract void AddActions();

    public virtual void Activate(Character user, Character target, bool isCrit)
    {
        SetupActions(user, target, isCrit);

        foreach (AbilityAction act in actions)
        {
            act.Execute(user, target, DoesHit(user, target, isCrit));
        }
    }

    public virtual bool DoesHit(Character user, Character target, bool isCrit)
    {
        return true;
    }

    public virtual void SetupActions(Character user, Character target, bool isCrit)
    {
        AddActions();

        foreach(ConditionalTuple ct in conditions) {
            if(ct.cond.CheckCondition(user, target))
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
        return $"<color=#FFBB00>{energyCost} Energy</color>" + FormatDescription(user) + (conditions.Count > 0 ? "\n" + conds : "");
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
            if (ct.cond.CheckCondition(user, target))
            {
                ct.effect.AddEffect(actions);
            }
        }

        if (isCrit)
        {
            critEffect.AddEffect(actions);
        }

        string actText = "";
        if (!DoesHit(user, target, isCrit))
        {
            actText += "<color=green>DODGED</color>\n";
        }
        else
        {
            for (int i = 0; i < actions.Count; i++)
            {
                AbilityAction act = actions[i];
                actText += act.GetActionText(user, target, DoesHit(user, target, isCrit));
                if (i < actions.Count - 1)
                {
                    actText += "\n";
                }
            }
        }

        target.DisplayActionPerm(actText);
    }

    public virtual (float, float) CalcActionValue(Character user, Character target, ActionValues mults)
    {
        AddActions();

        foreach (ConditionalTuple ct in conditions)
        {
            if (ct.cond.CheckCondition(user, target))
            {
                ct.effect.AddEffect(actions);
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