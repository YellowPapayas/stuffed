using UnityEngine;
using System.Collections.Generic;

public abstract class AccuracyAbility : Ability
{
    public int accuracy;
    private int finalAcc;

    public override void Activate(Character user, List<Character> targets, bool isCrit)
    {
        base.Activate(user, targets, isCrit);

        foreach (Character target in targets)
        {
            target.AccuracyCheck(GetFinalAccuracy(user, target, isCrit));
        }
    }

    public override string FormatDescription(Character user)
    {
        return base.FormatDescription(user) + $"\nACC: {accuracy}{user.GetStatString(StatType.Accuracy)}";
    }

    int GetFinalAccuracy(Character user, Character target, bool isCrit)
    {
        finalAcc = accuracy + user.GetStat(StatType.Accuracy);
        if (isCrit)
        {
            if (critEffect is AccuracyEffect accEff)
            {
                finalAcc += accEff.accMod;
            }
        }

        return finalAcc;
    }

    public override bool DoesHit(Character user, Character target, bool isCrit)
    {
        return GetFinalAccuracy(user, target, isCrit) > target.GetDodge();
    }

    public override (float, float) CalcActionValue(Character user, Character target, ActionValues mults)
    {
        (float reg, float crit) = base.CalcActionValue(user, target, mults);
        if (DoesHit(user, target, false))
        {
            return (reg, crit);
        } else
        {
            float removeDodge = (float) GetFinalAccuracy(user, target, false) * mults.removeValue / 5;
            float removeDodgeCrit = (float) GetFinalAccuracy(user, target, true) * mults.removeValue / 5;
            if (DoesHit(user, target, true))
            {
                return (removeDodge, crit);
            } else
            {
                return (removeDodge, removeDodgeCrit);
            }
        }
    }
}
