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

    public virtual void Activate(Character user, Character target, bool isCrit)
    {
        if (isCrit)
        {
            critEffect.AddEffect(user, target);
        }
    }

    public virtual string FormatDescription()
    {
        return $"<color=#FFBB00>{energyCost} Energy</color>";
    }

    public string CritDescription()
    {
        return FormatDescription() + "\n<color=#66AADD>" + critEffect.AddDescription() + "</color>";
    }

    public abstract string ActionText(Character user, Character target);

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
    MultiAlly
}
