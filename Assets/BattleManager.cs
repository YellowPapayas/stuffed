using UnityEngine;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour
{
    Character pendingChar;
    Ability pendingAbility;
    DescriptionText abilityDescription;
    DescriptionText energyDisplay;
    DescriptionText critDisplay;

    ClickHandle click;
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
        warning = GameObject.Find("Warning Text").GetComponent<TemporaryText>();
        abilityDescription = GameObject.Find("Description").GetComponent<DescriptionText>();
        energyDisplay = GameObject.Find("Energy Display").GetComponent<DescriptionText>();
        critDisplay = GameObject.Find("Crit Display").GetComponent<DescriptionText>();

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
                    target.DisplayAbilityAction(PreviewAbility(target));
                    pendingAbility.Activate(pendingChar, target, pendingCrit);
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
                }
            } else
            {
                warning.DisplayText("<color=#AA0000>Not enough energy!</color>", 300);
            }
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

    public string PreviewAbility(Character ch)
    {
        if(pendingChar != null && pendingAbility != null && CheckTarget(ch) && pendingChar.energy >= pendingAbility.energyCost)
        {
            return pendingAbility.ActionText(pendingChar, ch);
        } else
        {
            return "";
        }
    }

    bool TargetTeam()
    {
        switch (pendingAbility.targetType)
        {
            case TargetType.SingleEnemy:
            case TargetType.MultiEnemy:
                return !pendingChar.teamSide;
            case TargetType.SingleAlly:
            case TargetType.MultiAlly:
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
                return pendingChar.teamSide != target.teamSide;
            case TargetType.SingleAlly:
            case TargetType.MultiAlly:
                return pendingChar.teamSide == target.teamSide;
            default:
                return false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
