using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public abstract class TurnTaker
{
    public abstract void TakeTurn(Character character);
}

public struct ActionValues
{
    public float dmgValue;
    public float healValue;
    public float buffValue;
    public float debuffValue;
    public float removeValue;

    public ActionValues(float d, float h, float b, float db, float rv)
    {
        dmgValue = d;
        healValue = h;
        buffValue = b;
        debuffValue = db;
        removeValue = rv;
    }
}

public class PlayerTurn : TurnTaker
{
    public override void TakeTurn(Character character)
    {
        GameObject.Find("AbilityBar").GetComponent<AbilityBar>().DisplayAbilities(character);
        GameObject.Find("Energy Display").GetComponent<DescriptionText>().SetDescription(character.energy + " / " + character.stats.maxEnergy);
        GameObject.Find("Crit Display").GetComponent<DescriptionText>().SetDescription(character.currCrit + "");
    }
}

public class EnemyTurn : TurnTaker
{
    BattleManager bm;
    LineManager lm;
    DisplayManager dm;
    ActionValues values = new ActionValues(0.7f, 0.8f, 1f, 1f, 1f);

    List<AbilityOption> options;
    float maxValue;

    int energyAmount;
    int critAmount;
    
    public override void TakeTurn(Character character)
    {
        maxValue = 0f;
        bm = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        lm = GameObject.Find("BattleManager").GetComponent<LineManager>();
        dm = GameObject.Find("BattleManager").GetComponent<DisplayManager>();

        dm.ClearPlayerUI();

        List<AbilityOption> toCast = new List<AbilityOption>();
        energyAmount = character.energy;
        critAmount = character.currCrit;
        while(energyAmount > 0)
        {
            options = new List<AbilityOption>();
            foreach (Ability ab in character.abilities)
            {
                if (ab.energyCost <= energyAmount && ab.CanUse())
                {
                    options.AddRange(TargetValues(ab, character));
                }
            }

            float max = maxValue;
            foreach (AbilityOption abop in options)
            {
                float rand = Random.Range(0f, max);
                if (abop.value >= rand)
                {
                    toCast.Add(abop);
                    energyAmount -= abop.ability.energyCost;
                    if (abop.isCrit)
                    {
                        critAmount -= abop.ability.critCost;
                    }
                    break;
                } else
                {
                    max -= abop.value;
                }
            }
        }

        bm.StartCoroutine(CastAbilities(toCast));
    }

    IEnumerator CastAbilities(List<AbilityOption> toCast)
    {
        foreach (AbilityOption ab in toCast)
        {
            yield return new WaitForSeconds(1f);
            bm.pendingAbility = ab.ability;
            bm.pendingCrit = ab.isCrit;
            bm.PreviewAbility(ab.target);
            yield return bm.SelectTarget(ab.target);
            yield return new WaitForSeconds(0.8f);
        }
        yield return new WaitForSeconds(0.5f);
        dm.ShowPlayerUI();
        bm.NextTurn();
    }

    List<AbilityOption> TargetValues(Ability ab, Character user)
    {
        List<AbilityOption> output = new List<AbilityOption>();
        List<Character> groups = GetGroups(user, ab);
        foreach (Character ch in groups)
        {
            float reg = 0f;
            float crit = 0f;
            List<Character> targets = GetTargets(ch, ab);
            foreach (Character tar in targets)
            {
                (float a, float b) = ab.CalcActionValue(user, tar, values);
                reg += a;
                crit += b;
            }
            reg *= ((float)user.stats.maxEnergy) / ab.energyCost;
            crit *= ((float)user.stats.maxEnergy) / ab.energyCost;
            output.Add(new AbilityOption(ab, ch, reg, false));
            maxValue += reg;

            crit *= ((float) user.currCrit) / user.HighestCritCost();
            if (ab.critCost <= critAmount && crit != reg)
            {
                output.Add(new AbilityOption(ab, ch, crit, true));
                maxValue += crit;
            }
        }
        return output;
    }

    List<Character> GetGroups(Character ch, Ability ab)
    {
        switch (ab.targetType)
        {
            case TargetType.SingleEnemy:
                return bm.GetTeam(!ch.teamSide);
            case TargetType.SingleAlly:
                return bm.GetTeam(ch.teamSide);
            case TargetType.MultiEnemy:
                return lm.OneFromEach(!ch.teamSide);
            case TargetType.MultiAlly:
                return lm.OneFromEach(ch.teamSide);
            case TargetType.EnemyParty:
                List<Character> single = new List<Character>();
                single.Add(bm.GetTeam(!ch.teamSide)[0]);
                return single;
            case TargetType.AllyParty:
                List<Character> tingle = new List<Character>();
                tingle.Add(bm.GetTeam(ch.teamSide)[0]);
                return tingle;
        }
        return null;
    }

    List<Character> GetTargets(Character target, Ability ab)
    {
        switch (ab.targetType)
        {
            case TargetType.SingleEnemy:
            case TargetType.SingleAlly:
                List<Character> single = new List<Character>();
                single.Add(target);
                return single;
            case TargetType.MultiEnemy:
            case TargetType.MultiAlly:
                return lm.InSameLine(target);
            case TargetType.EnemyParty:
            case TargetType.AllyParty:
                return bm.GetTeam(target.teamSide);
            default:
                return null;
        }
    }
}

public struct AbilityOption
{
    public Ability ability;
    public Character target;
    public float value;
    public bool isCrit;

    public AbilityOption(Ability ab, Character tar, float val, bool crit)
    {
        ability = ab; target = tar; value = val; isCrit = crit;
    }

    public override string ToString()
    {
        return ability.abilityName + ", " + target.stats.characterName + ", " + value + ", " + isCrit;
    }
}