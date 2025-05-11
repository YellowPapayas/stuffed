using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewRemoveDebuffEffect", menuName = "CritEffects/RemoveDebuff")]
public class RemoveDebuffEffect : AbilityEffect
{
    public override void AddEffect(List<AbilityAction> actions)
    {
        actions.Add(new RemoveDebuffAction());
    }

    public override string AddDescription()
    {
        
        return "Remove all debuffs";
    }
}
