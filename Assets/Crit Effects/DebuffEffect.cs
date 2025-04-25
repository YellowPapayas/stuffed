using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewDebuffEffect", menuName = "CritEffects/Debuff")]
public class DebuffEffect : CritEffect
{
    public List<StatModifier> debuffs;

    public override void AddEffect(Character user, Character target)
    {
        foreach (StatModifier statMod in debuffs)
        {
            target.AddStatus(statMod.DeepCopy());
        }
    }

    public override string AddDescription()
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
        return base.AddDescription() + "\nApply " + listDebuffs;
    }

    public override void ActionText(Character user, Character target)
    {
        string action = "";
        foreach (StatModifier statMod in debuffs)
        {
            action += $"<color=#990000>-{statMod.type}</color>";
            action += "\n";
        }
        target.ActionAdd(action);
    }
}
