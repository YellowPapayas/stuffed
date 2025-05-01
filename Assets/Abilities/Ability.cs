using UnityEngine;
using System.Collections.Generic;

public abstract class Ability : ScriptableObject
{
    public string abilityName;
    public Sprite abilityImage;

    protected string abilityDescription;

    public TargetType targetType;
    public int energyCost;

    public CritEffect critEffect;
    public List<AbilityAction> actions;

    public abstract void AddActions();

    public void Activate(Character user, Character target, bool isCrit)
    {
        AddActions();

        if (isCrit)
        {
            critEffect.AddEffect(actions, user, target);
        }

        foreach (AbilityAction act in actions)
        {
            if(!act.Execute(user, target))
            {
                break;
            }
        }
    }

    public virtual string FormatDescription(Character user)
    {
        return $"<color=#FFBB00>{energyCost} Energy</color>";
    }

    public string CritDescription(Character user)
    {
        return FormatDescription(user) + "\n<color=#66AADD>" + critEffect.AddDescription() + "</color>";
    }

    public void ActionText(Character user, Character target, bool isCrit)
    {
        AddActions();

        if (isCrit)
        {
            critEffect.AddEffect(actions, user, target);
        }

        string actText = "";
        for (int i = 0; i < actions.Count; i++)
        {
            AbilityAction act = actions[i];
            actText += act.GetActionText(user, target);
            if(actText.Equals("<color=green>DODGED</color>"))
            {
                break;
            }
            if(i < actions.Count - 1)
            {
                actText += "\n";
            }
        }

        target.DisplayActionPerm(actText);
    }

    public (float, float) CalcActionValue(Character user, Character target, ActionValues mults)
    {
        AddActions();

        float total = 0f;
        foreach (AbilityAction act in actions)
        {
            total += act.GetActionValue(user, target, mults);
            if(act is DamageAction dmg)
            {
                if(!dmg.props.DoesHit(user, target))
                {
                    break;
                }
            }
            if (act is DebuffAction db)
            {
                if (!db.accProps.DoesHit(user, target))
                {
                    break;
                }
            }
            if (act is RemoveValueAction rv)
            {
                if (!rv.props.DoesHit(user, target))
                {
                    break;
                }
            }
        }

        critEffect.AddEffect(actions, user, target);

        float critTotal = 0f;
        foreach (AbilityAction act in actions)
        {
            critTotal += act.GetActionValue(user, target, mults);
            if (act is DamageAction dmg)
            {
                if (!dmg.props.DoesHit(user, target))
                {
                    break;
                }
            }
            if (act is DebuffAction db)
            {
                if (!db.accProps.DoesHit(user, target))
                {
                    break;
                }
            }
            if (act is RemoveValueAction rv)
            {
                if (!rv.props.DoesHit(user, target))
                {
                    break;
                }
            }
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

public enum TargetType
{
    SingleEnemy,
    SingleAlly,
    MultiEnemy,
    MultiAlly,
    EnemyParty,
    AllyParty
}