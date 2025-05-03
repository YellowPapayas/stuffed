using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewDamageEffect", menuName = "CritEffects/Damage")]
public class DamageEffect : AbilityEffect
{
    public float attackRatio;

    public override void AddEffect(List<AbilityAction> actions, Character user, Character target)
    {
        for (int i = 0; i < actions.Count; i++)
        {
            AbilityAction act = actions[i];
            if (act is DamageAction dmg)
            {
                dmg.attackRatio += attackRatio;
            }
        }
    }

    public override string AddDescription()
    {
        return $"Deal an additional {Mathf.FloorToInt(100 * attackRatio)}% ATK to the enemy/enemies";
    }
}
