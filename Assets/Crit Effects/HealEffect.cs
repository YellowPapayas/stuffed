using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewHealEffect", menuName = "CritEffects/Heal")]
public class HealEffect : AbilityEffect
{
    public float percentage;

    public override void AddEffect(List<AbilityAction> actions)
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
        HealAction healAct = new HealAction(percentage);
        healAct.targetSelf = this.targetSelf;
        actions.Add(healAct);
    }

    public override string AddDescription()
    {
        return $"Heal " + (targetSelf ? "self" : "the target") + $" for +{(int) (percentage*100)}% ATK";
    }
}
