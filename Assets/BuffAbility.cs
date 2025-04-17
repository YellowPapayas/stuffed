using UnityEngine;

[CreateAssetMenu(fileName = "NewBuffAbility", menuName = "Abilities/Buff")]
public class BuffAbility : Ability
{
    public StatType statType;
    public int amount;
    public int rounds;

    public override void Activate(Character user, Character target, bool isCrit)
    {
        StatModifier statMod = new StatModifier(statType, amount, rounds);
        target.AddStatus(statMod);

        base.Activate(user, target, isCrit);
    }

    public override string FormatDescription()
    {
        return base.FormatDescription() + "\n" + string.Format(abilityDescription, amount, statType, rounds);
    }

    public override string ActionText(Character user, Character target)
    {
        return $"<color=#009900>+{statType}</color>";
    }
}
