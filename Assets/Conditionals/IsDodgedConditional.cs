using UnityEngine;

[CreateAssetMenu(fileName = "IsDodgedConditional", menuName = "Conditionals/IsDodgedConditional")]
public class IsDodgedConditional : Conditional
{

    public override bool CheckCondition(Ability ability, Character user, Character target, bool isCrit)
    {
        return !ability.DoesHit(user, target, isCrit);
    }

    public override string ConditionString()
    {
        return "IF this ability is dodged:\n";
    }
}
