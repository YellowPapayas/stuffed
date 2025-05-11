using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewDamageAbility", menuName = "Abilities/Damage")]
public class DamageAbility : AccuracyAbility
{
    public float attackRatio;

    public override void AddActions()
    {
        actions = new List<AbilityAction>();
        actions.Add(new DamageAction(attackRatio));
    }

    public override string FormatDescription(Character user)
    {
        abilityDescription = "Deal <color=red>{0}%</color> ATK to " + StringTargetType();
        return base.FormatDescription(user) + "\n" + string.Format(abilityDescription, Mathf.FloorToInt(attackRatio*100));
    }
}
