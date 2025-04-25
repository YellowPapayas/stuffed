using UnityEngine;

public abstract class CritEffect : ScriptableObject
{
    public int critCost;

    public abstract void AddEffect(Character user, Character target);

    public virtual string AddDescription()
    {
        return $"<color=#66CCCC>{critCost} CRIT</color>";
    }

    public abstract void ActionText(Character user, Character target);
}
