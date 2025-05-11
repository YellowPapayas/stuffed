using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewDebuffAbility", menuName = "Abilities/Debuff")]
public class DebuffAbility : AccuracyAbility
{
    public List<StatModifier> debuffs;

    public override void AddActions()
    {
        actions = new List<AbilityAction>();
        actions.Add(new DebuffAction(debuffs));
    }

    public override string FormatDescription(Character user)
    {
        string listDebuffs = "";
        for (int i = 0; i < debuffs.Count; i++)
        {
            if (i == debuffs.Count - 1 && i != 0)
            {
                listDebuffs += " and ";
            }
            StatModifier statMod = debuffs[i];
            listDebuffs += $"<color=red>{statMod.amount}</color> <b>{statMod.type}</b> for <color=yellow>{statMod.rounds}</color> rounds";
            if (i < debuffs.Count - 1 && debuffs.Count > 2)
            {
                listDebuffs += ", ";
            }
        }
        abilityDescription = "Apply {0} to " + StringTargetType();
        return base.FormatDescription(user) + "\n" + string.Format(abilityDescription, listDebuffs);
    }
}
