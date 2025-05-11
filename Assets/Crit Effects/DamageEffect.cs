using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewDamageEffect", menuName = "CritEffects/Damage")]
public class DamageEffect : AbilityEffect
{
    public float attackRatio;

    public override void AddEffect(List<AbilityAction> actions)
    {
        for (int i = 0; i < actions.Count; i++)
        {
            AbilityAction act = actions[i];
            if (act is DamageAction dmg)
            {
                dmg.attackRatio += attackRatio;
                break;
            }
        }
        actions.Add(new DamageAction(attackRatio));
    }

    public override string AddDescription()
    {
        return $"Deal +{Mathf.FloorToInt(100 * attackRatio)}% ATK damage";
    }
}
