using UnityEngine;
using System.Collections.Generic;

public abstract class CritEffect : ScriptableObject
{
    public int critCost;

    public abstract void AddEffect(List<AbilityAction> actions, Character user, Character target);

    public virtual string AddDescription()
    {
        return $"<color=#66CCCC>{critCost} CRIT</color>";
    }
}
