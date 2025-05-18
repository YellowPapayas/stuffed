using UnityEngine;

[CreateAssetMenu(fileName = "NewToken", menuName = "Scriptable Objects/Token")]
public class TokenObject : ScriptableObject
{
    public string tokenName;

    public ActionCondition actionCondition;
    public int conditionAmount;
    public AbilityEffect effect;

    public void SetupIcon(TokenHover icon)
    {
        icon.description = $"<b><u>{tokenName}</u></b>\n" + ConditionString() + $" {conditionAmount} time(s):\n" + effect.AddDescription();
    }

    string ConditionString()
    {
        switch (actionCondition)
        {
            case ActionCondition.OnHit:
                return "Upon being hit";
            case ActionCondition.OnDodge:
                return "Upon dodging";
            case ActionCondition.OnBuffed:
                return "Upon being buffed";
            case ActionCondition.OnDebuffed:
                return "Upon being debuffed";
            case ActionCondition.OnHealed:
                return "Upon being healed";
            case ActionCondition.OnActivateToken:
                return "Upon activating this token";
            default:
                return "";
        }
    }
}

public enum ActionCondition
{
    OnHit,
    OnDodge,
    OnBuffed,
    OnDebuffed,
    OnHealed,
    OnActivateToken
}