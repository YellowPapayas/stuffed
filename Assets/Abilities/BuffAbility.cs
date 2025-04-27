using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewBuffAbility", menuName = "Abilities/Buff")]
public class BuffAbility : Ability
{
    public List<StatModifier> buffs;

    public override void AddActions()
    {
        actions = new List<AbilityAction>();
        actions.Add(new BuffAction(buffs));
    }

    public override string FormatDescription(Character user)
    {
        string listBuffs = "";
        for (int i = 0; i < buffs.Count; i++)
        {
            if (i == buffs.Count - 1 && i != 0)
            {
                listBuffs += " and ";
            }
            StatModifier statMod = buffs[i];
            listBuffs += $"<color=green>+{statMod.amount}</color> <b>{statMod.type}</b> for <color=yellow>{statMod.rounds}</color> rounds";
            if (i < buffs.Count - 1 && buffs.Count > 2)
            {
                listBuffs += ", ";
            }
        }
        return base.FormatDescription(user) + "\n" + string.Format(abilityDescription, listBuffs);
    }
}
