using UnityEngine;

[CreateAssetMenu(fileName = "NewHealEffect", menuName = "CritEffects/Heal")]
public class HealEffect : CritEffect
{
    public float percentage;

    public override void AddEffect(Character user, Character target)
    {
        user.OnHeal(Mathf.FloorToInt(user.stats.maxHealth * percentage));
    }

    public override string AddDescription()
    {
        return base.AddDescription() + $"\nHeal the user for {(int) (percentage*100)}% of their max health";
    }
}
