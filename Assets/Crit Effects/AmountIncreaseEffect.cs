using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewAmountIncreaseEffect", menuName = "CritEffects/AmountIncrease")]
public class AmountIncreaseEffect : AbilityEffect
{
    public int amount;

    public override void AddEffect(List<AbilityAction> actions, Character user, Character target)
    {
        for (int i = 0; i < actions.Count; i++)
        {
            AbilityAction act = actions[i];
            if (act is DebuffAction db)
            {
                foreach (StatModifier debuff in db.statMods)
                {
                    debuff.amount += amount;
                }
            }
        }
    }

    public override string AddDescription()
    {
        return $"Increase stat modifier amount(s) by {amount}";
    }
}
