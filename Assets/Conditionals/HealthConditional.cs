using UnityEngine;

[CreateAssetMenu(fileName = "NewHealthConditional", menuName = "Conditionals/Health")]
public class HealthConditional : Conditional
{
    public float healthPercent;
    public bool lessThan;

    public override bool CheckCondition(Character user, Character target)
    {
        Character toCheck = checkUser ? user : target;
        float currPercent = (float)toCheck.health / toCheck.stats.maxHealth;
        if ((lessThan && currPercent <= healthPercent) || (!lessThan && currPercent >= healthPercent))
        {
            return true;
        }
        return false;
    }

    public override string ConditionString()
    {
        return "IF " + (checkUser ? "user" : "target") + " has " + (lessThan ? "less than" : "more than") + $" or equal to {Mathf.FloorToInt(healthPercent * 100)} health:\n";
    }
}
