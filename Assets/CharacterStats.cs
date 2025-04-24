using UnityEngine;

[CreateAssetMenu(fileName = "CharacterStats", menuName = "Scriptable Objects/CharacterStats")]
public class CharacterStats : ScriptableObject
{
    public string characterName;
    public int maxHealth;
    public int attack;
    public int initiative;
    public int maxEnergy;
    public int crit;

    public int armor;
    public int dodge;
    public int accuracy;

    public int GetStat(StatType st)
    {
        switch (st)
        {
            case StatType.Attack: return attack;
            case StatType.Armor: return armor;
            case StatType.Initiative: return initiative;
            case StatType.Dodge: return dodge;
            case StatType.Accuracy: return accuracy;
            default: return 0;
        }
    }
}

public enum StatType
{
    Attack,
    Initiative,
    Armor,
    Dodge,
    Accuracy
}

[System.Serializable]
public class StatModifier
{
    public StatType type;
    public int amount;
    public int rounds;

    public StatModifier(StatType st, int am, int ro)
    {
        type = st; amount = am; rounds = ro;
    }

    public StatModifier DeepCopy()
    {
        return new StatModifier(type, amount, rounds);
    }
}