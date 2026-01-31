using UnityEngine;
using System.Collections.Generic;

public abstract class AbilityEffect : ScriptableObject
{
    public bool targetSelf;

    public abstract void AddEffect(List<AbilityAction> actions);

    public abstract string AddDescription();
}
