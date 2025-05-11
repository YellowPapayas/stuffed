using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewAccuracyEffect", menuName = "CritEffects/Accuracy")]
public class AccuracyEffect : AbilityEffect
{
    public int accMod;

    public override void AddEffect(List<AbilityAction> actions)
    {
        // do nothing
    }

    public override string AddDescription()
    {
        return $"Adds {accMod} ACC to the ability";
    }
}
