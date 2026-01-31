using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewActivateTokenEffect", menuName = "CritEffects/ActivateTokenEffect")]
public class ActivateTokenEffect : AbilityEffect
{
    public override void AddEffect(List<AbilityAction> actions)
    {
        ActivateTokenAction act = new ActivateTokenAction();
        act.targetSelf = this.targetSelf;
        actions.Add(act);
    }

    public override string AddDescription()
    {
        return "Activate tokens that require activation";
    }
}
