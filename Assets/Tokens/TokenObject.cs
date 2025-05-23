using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewToken", menuName = "Scriptable Objects/Token")]
public class TokenObject : ScriptableObject
{
    public string tokenName;
    public Sprite image;

    public ActionCondition actionCondition;
    public int conditionAmount;
    public AbilityEffect effect;

    public bool affectActivator;

    public void SetupIcon(TokenHover icon)
    {
        icon.description = $"<b><u>{tokenName}</u></b>\n" + ConditionString() + $" {conditionAmount} time(s):\n" + effect.AddDescription() + (affectActivator ? " to activator" : " to target");
        icon.SetImage(image);
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