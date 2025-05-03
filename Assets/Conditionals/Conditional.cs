using UnityEngine;

public abstract class Conditional : ScriptableObject
{
    public bool checkUser = false;

    public abstract bool CheckCondition(Character user, Character target);

    public abstract string ConditionString();
}