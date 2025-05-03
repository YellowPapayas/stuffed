using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewHealEffect", menuName = "CritEffects/Heal")]
public class HealEffect : AbilityEffect
{
    public float percentage;

    public override void AddEffect(List<AbilityAction> actions, Character user, Character target)
    {
        for (int i = 0; i < actions.Count; i++)
        {
            AbilityAction act = actions[i];
            if (act is HealAction heal)
            {
                heal.healRatio += percentage;
                return;
            }
        }
        actions.Add(new HealAction(percentage));
    }

    public override string AddDescription()
    {
        return $"Heal the target for {(int) (percentage*100)}% ATK";
    }
}
