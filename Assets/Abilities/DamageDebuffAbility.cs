using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewDamageDebuffAbility", menuName = "Abilities/DamageDebuff")]
public class DamageDebuffAbility : Ability
{
    public float attackRatio;
    public List<StatModifier> debuffs;
    public int accuracy;

    public override void AddActions()
    {
        actions = new List<AbilityAction>();
        actions.Add(new DamageAction(attackRatio, accuracy));
        actions.Add(new DebuffAction(debuffs, accuracy));
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
        abilityDescription = "Deal <color=red>{0}%</color> ATK and apply {1} to " + StringTargetType();
        return base.FormatDescription(user) + $"\nACC: {accuracy}{user.GetStatString(StatType.Accuracy)}\n" + string.Format(abilityDescription, Mathf.FloorToInt(attackRatio * 100), listDebuffs);
    }
}
