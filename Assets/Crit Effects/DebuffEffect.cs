using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewDebuffEffect", menuName = "CritEffects/Debuff")]
public class DebuffEffect : AbilityEffect
{
    public List<StatModifier> debuffs;

    public override void AddEffect(List<AbilityAction> actions)
    {
        for (int i = 0; i < actions.Count; i++)
        {
            AbilityAction act = actions[i];
            if (act is DebuffAction db)
            {
                db.statMods.AddRange(debuffs);
                return;
            }
        }
        actions.Add(new DebuffAction(debuffs));
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
        return "Apply " + listDebuffs;
    }
}
