using UnityEngine;

[CreateAssetMenu(fileName = "NewDamageAbility", menuName = "Abilities/Damage")]
public class DamageAbility : Ability
{
    public float attackRatio;
    public int accuracy;

    public override void Activate(Character user, Character target, bool isCrit)
    {
        bool didHit = target.OnHit(Mathf.FloorToInt(user.GetStat(StatType.Attack) * attackRatio), accuracy + user.GetStat(StatType.Accuracy));
        if (didHit)
        {
            base.Activate(user, target, isCrit);
        }
    }

    public override string FormatDescription(Character user)
    {
        return base.FormatDescription(user) + $"\nACC: {accuracy}{user.GetStatString(StatType.Accuracy)}\n" + string.Format(abilityDescription, Mathf.FloorToInt(attackRatio*100));
    }

    public override void ActionText(Character user, Character target, bool isCrit)
    {
        bool willHit;
        string output;
        if (accuracy + user.GetStat(StatType.Accuracy) > target.currDodge) {
            output = $"<color=red>{target.calcArmorDamage(Mathf.FloorToInt(user.GetStat(StatType.Attack) * attackRatio))}</color>";
            willHit = true;
        } else
        {
            output = $"<color=green>DODGED</color>";
            willHit = false;
        }
        target.DisplayActionPerm(output);
        if (willHit)
        {
            base.ActionText(user, target, isCrit);
        }
    }
}
