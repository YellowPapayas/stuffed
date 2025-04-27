using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewDamageAbility", menuName = "Abilities/Damage")]
public class DamageAbility : Ability
{
    public float attackRatio;
    public int accuracy;

    public override void AddActions()
    {
        actions = new List<AbilityAction>();
        actions.Add(new DamageAction(attackRatio, accuracy));
    }

    public override string FormatDescription(Character user)
    {
        return base.FormatDescription(user) + $"\nACC: {accuracy}{user.GetStatString(StatType.Accuracy)}\n" + string.Format(abilityDescription, Mathf.FloorToInt(attackRatio*100));
    }
}
