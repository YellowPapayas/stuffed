using UnityEngine;
using System.Collections.Generic;

public abstract class Ability : ScriptableObject
{
    public string abilityName;
    public Sprite abilityImage;

    [TextArea]
    public string abilityDescription;

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
            act.Execute(user, target);
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
            if(i < actions.Count - 1)
            {
                actText += "\n";
            }
        }

        target.DisplayActionPerm(actText);
    }

    public bool Equals(Ability other)
    {
        return other != null && abilityName == other.abilityName;
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