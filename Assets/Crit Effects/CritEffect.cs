using UnityEngine;
using System.Collections.Generic;

public abstract class AbilityEffect : ScriptableObject
{
    public abstract void AddEffect(List<AbilityAction> actions, Character user, Character target);

    public abstract string AddDescription();
}
