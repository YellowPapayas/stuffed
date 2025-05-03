using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewHealAbility", menuName = "Abilities/Heal")]
public class HealAbility : Ability
{
    public float healRatio;

    public override void AddActions()
    {
        actions = new List<AbilityAction>();
        actions.Add(new HealAction(healRatio));
    }

    public override string FormatDescription(Character user)
    {
        abilityDescription = "Heal <color=green>{0}%</color> ATK to " + StringTargetType();
        return base.FormatDescription(user) + "\n" + string.Format(abilityDescription, Mathf.FloorToInt(healRatio * 100));
    }
}
