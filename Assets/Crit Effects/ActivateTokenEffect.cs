using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewActivateTokenEffect", menuName = "CritEffects/ActivateTokenEffect")]
public class ActivateTokenEffect : AbilityEffect
{
    public override void AddEffect(List<AbilityAction> actions)
    {
        actions.Add(new ActivateTokenAction());
    }

    public override string AddDescription()
    {
        return "Activate tokens that require activation";
    }
}
