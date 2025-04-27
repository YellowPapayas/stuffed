using UnityEngine;
using System.Collections.Generic;

public abstract class AbilityAction
{
    public abstract void Execute(Character user, Character target);

    public abstract string GetActionText(Character user, Character target);
}

public class EnemyTargetProps
{
    public int accuracy;

    public int TotalAccuracy(Character user)
    {
        return accuracy + user.GetStat(StatType.Accuracy);
    }

    public bool DoesHit(Character user, Character target)
    {
        return accuracy + user.GetStat(StatType.Accuracy) > target.currDodge;
    }
}

public class DamageAction : AbilityAction
{
    public float attackRatio;
    public EnemyTargetProps props = new EnemyTargetProps();

    public DamageAction(float ratio, int accuracy)
    {
        this.attackRatio = ratio;
        props.accuracy = accuracy;
    }

    public override void Execute(Character user, Character target)
    {
        target.OnHit(Mathf.FloorToInt(user.GetStat(StatType.Attack) * attackRatio), props.TotalAccuracy(user));
    }

    public override string GetActionText(Character user, Character target)
    {
        if (props.DoesHit(user, target)) {
            return $"<color=red>{target.calcArmorDamage(Mathf.FloorToInt(user.GetStat(StatType.Attack) * attackRatio))}</color>";
        } else
        {
            return $"<color=green>DODGED</color>";
        }
    }
}

public class DebuffAction : AbilityAction
{
    public EnemyTargetProps accProps = new EnemyTargetProps();
    public List<StatModifier> statMods = new List<StatModifier>();

    public DebuffAction(List<StatModifier> mods, int accuracy)
    {
        statMods = new List<StatModifier>();
        statMods.AddRange(mods);

        accProps.accuracy = accuracy;
    }

    public override void Execute(Character user, Character target)
    {
        target.OnDebuffsHit(statMods, accProps.TotalAccuracy(user));
    }

    public override string GetActionText(Character user, Character target)
    {
        if (accProps.DoesHit(user, target))
        {
            string output = "";
            for (int i = 0; i < statMods.Count; i++)
            {
                StatModifier statMod = statMods[i];
                output += $"<color=#BB0000>-{statMod.type}</color>";
                if (i < statMods.Count - 1)
                {
                    output += "\n";
                }
            }
            return output;
        }
        else
        {
            return $"<color=green>DODGED</color>";
        }
    }
}

public class BuffAction : AbilityAction
{
    public List<StatModifier> statMods = new List<StatModifier>();

    public BuffAction(List<StatModifier> mods)
    {
        statMods = new List<StatModifier>();
        statMods.AddRange(mods);
    }

    public override void Execute(Character user, Character target)
    {
        foreach (StatModifier statMod in statMods)
        {
            target.AddStatus(statMod.DeepCopy());
        }
    }

    public override string GetActionText(Character user, Character target)
    {
        string output = "";
        for (int i = 0; i < statMods.Count; i++)
        {
            StatModifier statMod = statMods[i];
            output += $"<color=#00BB00>+{statMod.type}</color>";
            if (i < statMods.Count - 1)
            {
                output += "\n";
            }
        }
        return output;
    }
}

public class HealAction : AbilityAction
{
    public float healRatio;

    public HealAction(float ratio)
    {
        this.healRatio = ratio;
    }

    public override void Execute(Character user, Character target)
    {
        target.OnHeal(Mathf.FloorToInt(user.GetStat(StatType.Attack) * healRatio));
    }

    public override string GetActionText(Character user, Character target)
    {
        return $"<color=green>{Mathf.FloorToInt(user.GetStat(StatType.Attack) * healRatio)}</color>";
    }
}