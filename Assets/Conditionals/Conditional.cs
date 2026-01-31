using UnityEngine;

public abstract class Conditional : ScriptableObject
{
    public bool checkUser = false;

    public abstract bool CheckCondition(Ability ability, Character user, Character target, bool isCrit);

    public abstract string ConditionString();
}