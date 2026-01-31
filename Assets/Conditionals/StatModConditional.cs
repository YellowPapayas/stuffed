using UnityEngine;

[CreateAssetMenu(fileName = "NewStatModConditional", menuName = "Conditionals/StatMod")]
public class StatModConditional : Conditional
{
    public bool checkDebuff = true;

    public override bool CheckCondition(Ability ability, Character user, Character target, bool doesHit)
    {
        Character toCheck = checkUser ? user : target;
        foreach (StatModifier sm in toCheck.statMods)
        {
            if ((checkDebuff && sm.amount < 0) || (!checkDebuff && sm.amount > 0))
            {
                return true;
            }
        }
        return false;
    }

    public override string ConditionString()
    {
        return "IF " + (checkUser ? "user" : "target") + " has a " + (checkDebuff ? "debuff" : "buff") + ":\n";
    }
}
