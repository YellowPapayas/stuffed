using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewDebuffAbility", menuName = "Abilities/Debuff")]
public class DebuffAbility : Ability
{
    public List<StatModifier> debuffs;
    public int accuracy;

    public override void Activate(Character user, Character target, bool isCrit)
    {
        bool didHit = target.OnDebuffsHit(debuffs, accuracy + user.stats.accuracy);
        if (didHit)
        {
            base.Activate(user, target, isCrit);
        }
    }

    public override string FormatDescription()
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
        return base.FormatDescription() + "\n" + string.Format(abilityDescription, listDebuffs);
    }

    public override void ActionText(Character user, Character target, bool isCrit)
    {
        bool willHit;
        string output = "";
        if (accuracy + user.stats.accuracy > target.currDodge)
        {
            foreach (StatModifier statMod in debuffs)
            {
                output += $"<color=#990000>-{statMod.type}</color>";
                output += "\n";
            }
            willHit = true;
        }
        else
        {
            output = $"<color=green>DODGED</color>";
            willHit = false;
        }
        target.DisplayActionPerm(output);
        if (willHit)
        {
            base.ActionText(user, target, isCrit);
        }
    }
}
