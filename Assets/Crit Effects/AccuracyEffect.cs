using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewAccuracyEffect", menuName = "CritEffects/Accuracy")]
public class AccuracyEffect : CritEffect
{
    public int accMod;

    public override void AddEffect(List<AbilityAction> actions, Character user, Character target)
    {
        for (int i = 0; i < actions.Count; i++)
        {
            AbilityAction act = actions[i];
            if (act is DamageAction dmg)
            {
                dmg.props.accuracy += accMod;
            }
            if (act is DebuffAction db)
            {
                db.accProps.accuracy += accMod;
            }
        }
    }

    public override string AddDescription()
    {
        return base.AddDescription() + $"\nAdds {accMod} ACC to the ability";
    }
}
