using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewRemoveDebuffEffect", menuName = "CritEffects/RemoveDebuff")]
public class RemoveDebuffEffect : AbilityEffect
{
    public override void AddEffect(List<AbilityAction> actions)
    {
        RemoveDebuffAction rmdbAct = new RemoveDebuffAction();
        rmdbAct.targetSelf = this.targetSelf;
        actions.Add(rmdbAct);
    }

    public override string AddDescription()
    {
        
        return "Remove all debuffs";
    }
}
