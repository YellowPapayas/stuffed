using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ComboAbility", menuName = "Abilities/ComboAbility")]
public class ComboAbility : Ability
{
    public List<AbilityEffect> effects;

    public override void AddActions()
    {
        actions = new List<AbilityAction>();
        foreach (AbilityEffect abEff in effects)
        {
            abEff.AddEffect(actions);
        }
    }

    public override string FormatDescription(Character user)
    {
        abilityDescription = "";
        for (int i = 0; i < effects.Count; i++)
        {
            AbilityEffect abEff = effects[i];
            abilityDescription += abEff.AddDescription();
            if (i < effects.Count - 1)
            {
                abilityDescription += "\n";
            }
        }
        return base.FormatDescription(user) + "\n" + abilityDescription;
    }
}
