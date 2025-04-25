using UnityEngine;

[CreateAssetMenu(fileName = "NewDamageEffect", menuName = "CritEffects/Damage")]
public class DamageEffect : CritEffect
{
    public float attackRatio;

    public override void AddEffect(Character user, Character target)
    {
        target.OnHit(Mathf.FloorToInt(user.GetStat(StatType.Attack) * attackRatio), target.currDodge + 100);
    }

    public override string AddDescription()
    {
        return base.AddDescription() + $"\nDeal an additional {Mathf.FloorToInt(100 * attackRatio)}% ATK to the enemy/enemies";
    }

    public override void ActionText(Character user, Character target)
    {
        target.ActionAdd($"<color=red>{target.calcArmorDamage(Mathf.FloorToInt(user.GetStat(StatType.Attack) * attackRatio))}</color>");
    }
}
