using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewBuffEffect", menuName = "CritEffects/Buff")]
public class BuffEffect : AbilityEffect
{
    public List<StatModifier> buffs;

    public override void AddEffect(List<AbilityAction> actions)
    {
        for (int i = 0; i < actions.Count; i++)
        {
            AbilityAction act = actions[i];
            if (act is BuffAction ba)
            {
                ba.statMods.AddRange(buffs);
                break;
            }
        }
        actions.Add(new BuffAction(buffs));
    }

    public override string AddDescription()
    {
        string listBuffs = "";
        for (int i = 0; i < buffs.Count; i++)
        {
            if (i == buffs.Count - 1 && i != 0)
            {
                listBuffs += " and ";
            }
            StatModifier statMod = buffs[i];
            listBuffs += $"<color=green>{statMod.amount}</color> <b>{statMod.type}</b> for <color=yellow>{statMod.rounds}</color> rounds";
            if (i < buffs.Count - 1 && buffs.Count > 2)
            {
                listBuffs += ", ";
            }
        }
        return "Apply " + listBuffs;
    }
}
