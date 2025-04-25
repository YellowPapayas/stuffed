using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewBuffAbility", menuName = "Abilities/Buff")]
public class BuffAbility : Ability
{
    public List<StatModifier> buffs;

    public override void Activate(Character user, Character target, bool isCrit)
    {
        foreach (StatModifier statMod in buffs)
        {
            target.AddStatus(statMod.DeepCopy());
        }

        base.Activate(user, target, isCrit);
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

    public override void ActionText(Character user, Character target, bool isCrit)
    {
        string action = "";
        foreach (StatModifier statMod in buffs)
        {
            action += $"<color=#009900>+{statMod.type}</color>";
            action += "\n";
        }
        target.DisplayActionPerm(action);
        base.ActionText(user, target, isCrit);
    }
}
