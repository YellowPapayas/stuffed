using UnityEngine;
using System.Collections.Generic;

public abstract class AbilityAction
{
    public abstract void Execute(Character user, Character target, bool doesHit);

    public abstract string GetActionText(Character user, Character target, bool doesHit);

    public abstract float GetActionValue(Character user, Character target, bool doesHit, ActionValues mults);
}

public class DamageAction : AbilityAction
{
    public float attackRatio;

    public DamageAction(float ratio)
    {
        this.attackRatio = ratio;
    }

    public override void Execute(Character user, Character target, bool doesHit)
    {
        if (doesHit)
        {
            target.OnHit(Mathf.FloorToInt(user.GetStat(StatType.Attack) * attackRatio));
        }
    }

    public override string GetActionText(Character user, Character target, bool doesHit)
    {
        if (doesHit)
        {
            return $"<color=red>{target.calcArmorDamage(Mathf.FloorToInt(user.GetStat(StatType.Attack) * attackRatio))}</color>";
        } else
        {
            return "";
        }
    }

    public override float GetActionValue(Character user, Character target, bool doesHit, ActionValues mults)
    {
        if (doesHit)
        {
            int calcDmg = target.calcArmorDamage(Mathf.FloorToInt(user.GetStat(StatType.Attack) * attackRatio));
            int targetMaxHealth = target.stats.maxHealth;

            float percentDmg = 100 * ((float)calcDmg) / targetMaxHealth;
            return percentDmg * mults.dmgValue;
        } else
        {
            return 0;
        }
    }
}

public class DebuffAction : AbilityAction
{
    public List<StatModifier> statMods = new List<StatModifier>();

    public DebuffAction(List<StatModifier> mods)
    {
        statMods = new List<StatModifier>();
        foreach (StatModifier sm in mods)
        {
            statMods.Add(sm.DeepCopy());
        }
    }

    public override void Execute(Character user, Character target, bool doesHit)
    {
        if (doesHit)
        {
            target.OnDebuffsHit(statMods);
        }
    }

    public override string GetActionText(Character user, Character target, bool doesHit)
    {
        if (doesHit)
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
        } else
        {
            return "";
        }
    }

    public override float GetActionValue(Character user, Character target, bool doesHit, ActionValues mults)
    {
        if (doesHit)
        {
            float output = 0f;
            foreach (StatModifier sm in statMods)
            {
                int stat = target.GetStat(sm.type);
                if (stat == 0)
                {
                    stat = 1;
                }
                float percentDebuff = sm.rounds * ((float)sm.amount) / stat;
                if (percentDebuff < 0)
                {
                    percentDebuff *= -1;
                }
                output += percentDebuff * mults.debuffValue;
            }
            return output;
        } else
        {
            return 0f;
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

    public override void Execute(Character user, Character target, bool doesHit)
    {
        foreach (StatModifier statMod in statMods)
        {
            if(user.Equals(target))
            {
                statMod.removeThisRound = false;
            }
            target.AddStatus(statMod.DeepCopy());
        }
    }

    public override string GetActionText(Character user, Character target, bool doesHit)
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

    public override float GetActionValue(Character user, Character target, bool doesHit, ActionValues mults)
    {
        float output = 0f;
        foreach (StatModifier sm in statMods)
        {
            int stat = target.GetStat(sm.type);
            if (stat == 0)
            {
                stat = 1;
            }
            float percentBuff = sm.rounds * ((float)sm.amount) / stat;
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

    public override void Execute(Character user, Character target, bool doesHit)
    {
        target.OnHeal(Mathf.FloorToInt(user.GetStat(StatType.Attack) * healRatio));
    }

    public override string GetActionText(Character user, Character target, bool doesHit)
    {
        return $"<color=green>{Mathf.FloorToInt(user.GetStat(StatType.Attack) * healRatio)}</color>";
    }

    public override float GetActionValue(Character user, Character target, bool doesHit, ActionValues mults)
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

    public RemoveValueAction(List<ValueModifier> mods)
    {
        valueMods = mods;
    }

    public override void Execute(Character user, Character target, bool doesHit)
    {
        if (doesHit)
        {
            target.OnRemoveValue(valueMods);
        }
    }

    public override string GetActionText(Character user, Character target, bool doesHit)
    {
        if (doesHit)
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
        } else
        {
            return "";
        }
    }

    public override float GetActionValue(Character user, Character target, bool doesHit, ActionValues mults)
    {
        if (doesHit)
        {
            float output = 0f;
            foreach (ValueModifier vm in valueMods)
            {
                int diff = target.GetCurrentValue(vm.value) + vm.amount;
                float percentRemove = (((float)vm.amount) / diff);
                if (percentRemove < 0)
                {
                    percentRemove *= -1f;
                }
                output += percentRemove * mults.removeValue;
            }
            return output;
        } else
        {
            return 0;
        }
    }
}

public class AddValueAction : AbilityAction
{
    public List<ValueModifier> valueMods = new List<ValueModifier>();

    public AddValueAction(List<ValueModifier> mods)
    {
        valueMods = mods;
    }

    public override void Execute(Character user, Character target, bool doesHit)
    {
        target.OnAddValue(valueMods);
    }

    public override string GetActionText(Character user, Character target, bool doesHit)
    {
        string output = "";
        for (int i = 0; i < valueMods.Count; i++)
        {
            ValueModifier valMod = valueMods[i];
            output += $"<color=#00BB55>+CURRENT {valMod.value}</color>";
            if (i < valueMods.Count - 1)
            {
                output += "\n";
            }
        }
        return output;
    }

    public override float GetActionValue(Character user, Character target, bool doesHit, ActionValues mults)
    {
        float output = 0f;
        foreach (ValueModifier vm in valueMods)
        {
            int diff = target.GetCurrentValue(vm.value) + vm.amount;
            float percentRemove = (((float)vm.amount) / diff);
            if (percentRemove < 0)
            {
                percentRemove *= -1f;
            }
            output += percentRemove * mults.removeValue;
        }
        return output;
    }
}

public class RemoveDebuffAction : AbilityAction
{
    public override void Execute(Character user, Character target, bool doesHit)
    {
        for (int i = target.statMods.Count - 1; i >= 0; i--)
        {
            if (target.statMods[i].amount < 0)
            {
                target.RemoveStatusIndex(i);
            }
        }
    }

    public override string GetActionText(Character user, Character target, bool doesHit)
    {
        return $"<color=green>-DEBUFFS</color>";
    }

    public override float GetActionValue(Character user, Character target, bool doesHit, ActionValues mults)
    {
        float output = 0f;
        foreach (StatModifier sm in target.statMods)
        {
            if(sm.amount < 0)
            {
                int stat = target.stats.GetStat(sm.type);
                if(stat == 0)
                {
                    stat = 1;
                }
                float percent = sm.rounds * ((float) sm.amount / stat);
                if(percent < 0)
                {
                    percent *= -1;
                }
                output += percent * mults.debuffValue;
            }
        }
        return output;
    }
}

public class AddTokenAction : AbilityAction
{
    public List<TokenObject> tokens = new List<TokenObject>();

    public AddTokenAction(List<TokenObject> tokenList)
    {
        tokens.AddRange(tokenList);
    }

    public override void Execute(Character user, Character target, bool doesHit)
    {
        if (doesHit)
        {
            foreach (TokenObject token in tokens)
            {
                target.AddToken(token, user);
            }
        }
    }

    public override string GetActionText(Character user, Character target, bool doesHit)
    {
        if(!doesHit)
        {
            return "";
        }
        string output = "<color=yellow>";
        for(int i = 0; i < tokens.Count; i++)
        {
            TokenObject token = tokens[i];
            output += $"+{token.tokenName} TOKEN";
            if(i < tokens.Count - 1)
            {
                output += "\n";
            }
        }
        return output;
    }

    public override float GetActionValue(Character user, Character target, bool doesHit, ActionValues mults)
    {
        if (!doesHit)
        {
            return 0f;
        }
        float output = 0f;
        foreach (TokenObject token in tokens)
        {
            List<AbilityAction> tokenActions = new List<AbilityAction>();
            token.effect.AddEffect(tokenActions);
            foreach (AbilityAction action in tokenActions)
            {
                output += action.GetActionValue(user, target, doesHit, mults);
            }
        }
        return output;
    }
}

public class ActivateTokenAction : AbilityAction
{
    public override void Execute(Character user, Character target, bool doesHit)
    {
        if (doesHit)
        {
            target.OnActivateToken();
        }
    }

    public override string GetActionText(Character user, Character target, bool doesHit)
    {
        string output = "";
        foreach (TokenTuple token in target.tokens)
        {
            List<AbilityAction> tokenActions = new List<AbilityAction>();
            token.effect.AddEffect(tokenActions);
            foreach (AbilityAction action in tokenActions)
            {
                output += action.GetActionText(user, target, doesHit);
            }
        }
        return output;
    }

    public override float GetActionValue(Character user, Character target, bool doesHit, ActionValues mults)
    {
        float output = 0f;
        foreach (TokenTuple token in target.tokens)
        {
            List<AbilityAction> tokenActions = new List<AbilityAction>();
            token.effect.AddEffect(tokenActions);
            foreach (AbilityAction action in tokenActions)
            {
                output += action.GetActionValue(user, target, doesHit, mults);
            }
        }
        return output;
    }
}