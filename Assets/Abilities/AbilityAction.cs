using UnityEngine;
using System.Collections.Generic;

public abstract class AbilityAction
{
    public abstract bool Execute(Character user, Character target);

    public abstract string GetActionText(Character user, Character target);

    public abstract float GetActionValue(Character user, Character target, ActionValues mults);
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

    public override bool Execute(Character user, Character target)
    {
        return target.OnHit(Mathf.FloorToInt(user.GetStat(StatType.Attack) * attackRatio), props.TotalAccuracy(user));
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

    public override float GetActionValue(Character user, Character target, ActionValues mults)
    {
        if (props.DoesHit(user, target))
        {
            int calcDmg = target.calcArmorDamage(Mathf.FloorToInt(user.GetStat(StatType.Attack) * attackRatio));
            int targetMaxHealth = target.stats.maxHealth;

            float percentDmg = 100 * ((float)calcDmg) / targetMaxHealth;
            return percentDmg * mults.dmgValue;
        } else
        {
            float percentDodge = 10 * ((float)props.accuracy) / target.currDodge;
            return percentDodge * mults.removeValue;
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
        foreach (StatModifier sm in mods)
        {
            statMods.Add(sm.DeepCopy());
        }

        accProps.accuracy = accuracy;
    }

    public override bool Execute(Character user, Character target)
    {
        return target.OnDebuffsHit(statMods, accProps.TotalAccuracy(user));
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

    public override float GetActionValue(Character user, Character target, ActionValues mults)
    {
        if (accProps.DoesHit(user, target))
        {
            float output = 0f;
            foreach (StatModifier sm in statMods)
            {
                int diff = target.GetStat(sm.type) + sm.amount;
                float percentDebuff = sm.rounds * ((float)sm.amount) / diff;
                if(percentDebuff < 0)
                {
                    percentDebuff *= -1;
                }
                output += percentDebuff * mults.debuffValue;
            }
            return output;
        } else
        {
            float percentDodge = 10 * ((float)accProps.accuracy) / target.currDodge;
            return percentDodge * mults.removeValue;
        }
    }
}

public class BuffAction : AbilityAction
{
    public List<StatModifier> statMods = new List<StatModifier>();

    public BuffAction(List<StatModifier> mods)
    {
        statMods = new List<StatModifier>();
        foreach (StatModifier sm in mods)
        {
            statMods.Add(sm.DeepCopy());
        }
    }

    public override bool Execute(Character user, Character target)
    {
        foreach (StatModifier statMod in statMods)
        {
            target.AddStatus(statMod.DeepCopy());
        }
        return true;
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

    public override float GetActionValue(Character user, Character target, ActionValues mults)
    {
        float output = 0f;
        foreach (StatModifier sm in statMods)
        {
            int diff = target.GetStat(sm.type) + sm.amount;
            float percentBuff = sm.rounds * ((float)sm.amount) / diff;
            if(percentBuff < 0)
            {
                percentBuff *= -1;
            }
            output += percentBuff * mults.buffValue;
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

    public override bool Execute(Character user, Character target)
    {
        target.OnHeal(Mathf.FloorToInt(user.GetStat(StatType.Attack) * healRatio));
        return true;
    }

    public override string GetActionText(Character user, Character target)
    {
        return $"<color=green>{Mathf.FloorToInt(user.GetStat(StatType.Attack) * healRatio)}</color>";
    }

    public override float GetActionValue(Character user, Character target, ActionValues mults)
    {
        float calcHeal = Mathf.FloorToInt(user.GetStat(StatType.Attack)) * healRatio;
        int targetMaxHealth = target.stats.maxHealth;

        float percentHeal = 100 * calcHeal / targetMaxHealth;
        return percentHeal * mults.dmgValue;
    }
}

public class RemoveValueAction : AbilityAction
{
    public List<ValueModifier> valueMods = new List<ValueModifier>();
    public EnemyTargetProps props = new EnemyTargetProps();

    public RemoveValueAction(List<ValueModifier> mods, int acc)
    {
        valueMods = mods;
        props.accuracy = acc;
    }

    public override bool Execute(Character user, Character target)
    {
        return target.OnRemoveValue(valueMods, props.TotalAccuracy(user));
    }

    public override string GetActionText(Character user, Character target)
    {
        if (props.DoesHit(user, target))
        {
            string output = "";
            for (int i = 0; i < valueMods.Count; i++)
            {
                ValueModifier valMod = valueMods[i];
                output += $"<color=#BB0055>-CURRENT {valMod.value}</color>";
                if (i < valueMods.Count - 1)
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

    public override float GetActionValue(Character user, Character target, ActionValues mults)
    {
        if (props.DoesHit(user, target))
        {
            float output = 0f;
            foreach (ValueModifier vm in valueMods)
            {
                int diff = target.GetCurrentValue(vm.value) + vm.amount;
                float percentRemove = (((float)vm.amount) / diff);
                if(percentRemove < 0)
                {
                    percentRemove *= -1f;
                }
                output += percentRemove * mults.removeValue;
            }
            return output;
        } else
        {
            float percentDodge = 10 * ((float)props.accuracy) / target.currDodge;
            return percentDodge * mults.removeValue;
        }
    }
}