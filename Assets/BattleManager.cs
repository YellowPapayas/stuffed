using UnityEngine;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour
{
    Character pendingChar;
    Ability pendingAbility;
    DescriptionText abilityDescription;
    DescriptionText energyDisplay;
    DescriptionText critDisplay;
    TurnOrderDisplay turnOrderDisplay;

    ClickHandle click;
    LineManager lm;
    TemporaryText warning;

    List<Character> characters;
    List<Character> leftSide;
    List<Character> rightSide;
    Queue<Character> turnQueue;

    bool pendingCrit = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        click = GameObject.Find("ClickHandler").GetComponent<ClickHandle>();
        lm = GetComponent<LineManager>();
        warning = GameObject.Find("Warning Text").GetComponent<TemporaryText>();
        abilityDescription = GameObject.Find("Description").GetComponent<DescriptionText>();
        abilityDescription.Setup();
        energyDisplay = GameObject.Find("Energy Display").GetComponent<DescriptionText>();
        energyDisplay.Setup();
        critDisplay = GameObject.Find("Crit Display").GetComponent<DescriptionText>();
        critDisplay.Setup();
        turnOrderDisplay = GameObject.Find("Turn Order").GetComponent<TurnOrderDisplay>();

        characters = new List<Character>();
        leftSide = new List<Character>();
        rightSide = new List<Character>();
        AddCharacters();

        turnQueue = new Queue<Character>();
        NextRound();
    }

    void AddCharacters()
    {
        Character[] charArr = FindObjectsByType<Character>(FindObjectsSortMode.None);
        foreach (Character c in charArr)
        {
            characters.Add(c);
            if (c.teamSide)
            {
                leftSide.Add(c);
            } else
            {
                rightSide.Add(c);
            }
        }

        turnOrderDisplay.Setup(characters);
    }

    List<Character> GetTeam(bool teamSide)
    {
        if (teamSide)
        {
            return leftSide;
        }
        else { 
            return rightSide;
        }
    }

    void HighlightCharacters(List<Character> toHighlight, bool show)
    {
        foreach (Character c in toHighlight)
        {
            c.SetHighlight(show);
        }
    }

    void ClearActionText(List<Character> toClear)
    {
        foreach (Character c in toClear)
        {
            c.ActionOff();
        }
    }

    void SetActionDuration(List<Character> toSet, int duration)
    {
        foreach (Character c in toSet)
        {
            c.ActionDuration(duration);
        }
    }

    void QueueTurns()
    {
        List<Character> toAdd = new List<Character>();
        toAdd.AddRange(characters);
        for (int i = 0; i < characters.Count; i++)
        {
            Character highest = null;
            foreach (Character ch in toAdd)
            {
                if (highest == null || ch.stats.initiative > highest.stats.initiative)
                {
                    highest = ch;
                }
            }
            turnQueue.Enqueue(highest);
            toAdd.Remove(highest);
        }

        turnOrderDisplay.AddAllIcons(turnQueue);
    }

    void NextRound()
    {
        QueueTurns();
        NextTurn();
    }

    public void NextTurn()
    {
        if (pendingChar != null)
        {
            pendingChar.OnTurnEnd();
            foreach (Character ch in characters)
            {
                ch.OnGeneralTurnEnd();
            }
            pendingChar = null;

            turnOrderDisplay.RemoveTopTurn();
        }
        if (turnQueue.Count > 0)
        {
            pendingChar = turnQueue.Dequeue();
            pendingChar.OnTurnStart();
            GameObject.Find("AbilityBar").GetComponent<AbilityBar>().DisplayAbilities(pendingChar);
            energyDisplay.SetDescription(pendingChar.energy + " / " + pendingChar.stats.maxEnergy);
            critDisplay.SetDescription(pendingChar.currCrit + "");
        }
        else
        {
            NextRound();
        }
    }

    public void StartTargeting(Ability ability)
    {
        if (ability.Equals(pendingAbility))
        {
            pendingCrit = !pendingCrit;
        } else
        {
            pendingCrit = false;
        }

        pendingAbility = ability;
        HighlightCharacters(GetTeam(TargetTeam()), true);
        if (pendingCrit)
        {
            abilityDescription.SetDescription(pendingAbility.CritDescription());
        }
        else
        {
            abilityDescription.SetDescription(pendingAbility.FormatDescription());
        }

        click.selected = true;
    }

    public void SelectTarget(Character target)
    {
        if (CheckTarget(target))
        {
            if (pendingChar.energy >= pendingAbility.energyCost)
            {
                if (!pendingCrit || pendingChar.currCrit >= pendingAbility.critEffect.critCost)
                {
                    List<Character> targets = GetTargets(target);
                    PreviewAbility(target);
                    SetActionDuration(characters, 300);
                    foreach (Character ch in targets)
                    {
                        pendingAbility.Activate(pendingChar, ch, pendingCrit);
                    }
                    pendingChar.energy -= pendingAbility.energyCost;
                    energyDisplay.SetDescription(pendingChar.energy + " / " + pendingChar.stats.maxEnergy);
                    if (pendingCrit)
                    {
                        pendingChar.currCrit -= pendingAbility.critEffect.critCost;
                        critDisplay.SetDescription(pendingChar.currCrit + "");
                    }
                } else
                {
                    warning.DisplayText("<color=#AA0000>Not enough crit!</color>", 300);
                    ClearActionText(characters);
                }
            } else
            {
                warning.DisplayText("<color=#AA0000>Not enough energy!</color>", 300);
                ClearActionText(characters);
            }
        } else
        {
            ClearActionText(characters);
        }
        pendingAbility = null;
        StopTargeting();
    }

    public void StopTargeting()
    {
        HighlightCharacters(characters, false);
        abilityDescription.SetDescription("");

        click.selected = false;
    }

    public List<Character> PreviewAbility(Character ch)
    {
        if(pendingChar != null && pendingAbility != null && CheckTarget(ch) && pendingChar.energy >= pendingAbility.energyCost)
        {
            List<Character> toPreview = GetTargets(ch);
            foreach (Character tar in toPreview)
            {
                pendingAbility.ActionText(pendingChar, tar, pendingCrit);
            }

            toPreview.Add(pendingChar);
            return toPreview;
        }
        return null;
    }

    bool TargetTeam()
    {
        switch (pendingAbility.targetType)
        {
            case TargetType.SingleEnemy:
            case TargetType.MultiEnemy:
            case TargetType.EnemyParty:
                return !pendingChar.teamSide;
            case TargetType.SingleAlly:
            case TargetType.MultiAlly:
            case TargetType.AllyParty:
                return pendingChar.teamSide;
            default:
                return false;
        }
    }

    bool CheckTarget(Character target)
    {
        switch (pendingAbility.targetType) {
            case TargetType.SingleEnemy:
            case TargetType.MultiEnemy:
            case TargetType.EnemyParty:
                return pendingChar.teamSide != target.teamSide;
            case TargetType.SingleAlly:
            case TargetType.MultiAlly:
            case TargetType.AllyParty:
                return pendingChar.teamSide == target.teamSide;
            default:
                return false;
        }
    }

    List<Character> GetTargets(Character target)
    {
        switch (pendingAbility.targetType)
        {
            case TargetType.SingleEnemy:
            case TargetType.SingleAlly:
                List<Character> single = new List<Character>();
                single.Add(target);
                return single;
            case TargetType.MultiEnemy:
            case TargetType.MultiAlly:
                return lm.InSameLine(target);
            case TargetType.EnemyParty:
            case TargetType.AllyParty:
                return GetTeam(target.teamSide);
            default:
                return characters;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
