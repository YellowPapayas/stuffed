using UnityEngine;

[CreateAssetMenu(fileName = "NewDamageAbility", menuName = "Abilities/Damage")]
public class DamageAbility : Ability
{
    public float attackRatio;
    public int accuracy;

    public override void Activate(Character user, Character target, bool isCrit)
    {
        target.OnHit(Mathf.FloorToInt(user.GetStat(StatType.Attack) * attackRatio), accuracy);
        base.Activate(user, target, isCrit);
    }

    public override string FormatDescription()
    {
        return base.FormatDescription() + "\n" + string.Format(abilityDescription, Mathf.FloorToInt(attackRatio*100));
    }

    public override void ActionText(Character user, Character target, bool isCrit)
    {
        string output;
        if (accuracy > target.currDodge) {
            output = $"<color=red>{target.calcArmorDamage(Mathf.FloorToInt(user.GetStat(StatType.Attack) * attackRatio))}</color>";
        } else
        {
            output = $"<color=green>DODGED</color>";
        }
        target.DisplayActionPerm(output);
        base.ActionText(user, target, isCrit);
    }
}
