using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewRemoveValueEffect", menuName = "CritEffects/RemoveValue")]
public class RemoveValueEffect : AbilityEffect
{
    public List<ValueModifier> valueMods;

    public override void AddEffect(List<AbilityAction> actions, Character user, Character target)
    {
        for (int i = 0; i < actions.Count; i++)
        {
            AbilityAction act = actions[i];
            if (act is DamageAction dmg)
            {
                if (dmg.props.DoesHit(user, target))
                {
                    actions.Add(new RemoveValueAction(valueMods, target.currDodge + 100));
                }
                break;
            }
            if (act is DebuffAction db)
            {
                if (db.accProps.DoesHit(user, target))
                {
                    actions.Add(new RemoveValueAction(valueMods, target.currDodge + 100));
                }
                break;
            }
            if (act is RemoveValueAction rv)
            {
                rv.valueMods.AddRange(valueMods);
                break;
            }
        }
    }

    public override string AddDescription()
    {
        string listValueMods = "";
        for (int i = 0; i < valueMods.Count; i++)
        {
            if (i == valueMods.Count - 1 && i != 0)
            {
                listValueMods += " and ";
            }
            ValueModifier valMod = valueMods[i];
            listValueMods += $"<color=red>{valMod.amount}</color> <b>{valMod.value}</b> value";
            if (i < valueMods.Count - 1 && valueMods.Count > 2)
            {
                listValueMods += ", ";
            }
        }
        return "Remove " + listValueMods;
    }
}
