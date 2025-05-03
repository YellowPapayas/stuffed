using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class BattleManager : MonoBehaviour
{
    Character pendingChar;
    public Ability pendingAbility;
    DescriptionText abilityDescription;
    DescriptionText energyDisplay;
    DescriptionText critDisplay;
    TurnOrderDisplay turnOrderDisplay;

    ClickHandle click;
    LineManager lm;
    DisplayManager dm;
    TemporaryText warning;

    List<Character> characters;
    List<Character> leftSide;
    List<Character> rightSide;
    Queue<Character> turnQueue;

    public bool pendingCrit = false;
    public Ability lastClickedAbility = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        click = GameObject.Find("ClickHandler").GetComponent<ClickHandle>();
        lm = GetComponent<LineManager>();
        dm = GetComponent<DisplayManager>();
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

    public void OnCharacterDeath(Character ch)
    {
        GetTeam(ch.teamSide).Remove(ch);
        characters.Remove(ch);
    }

    public List<Character> GetTeam(bool teamSide)
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
            if (pendingChar == null)
            {
                turnOrderDisplay.RemoveTopTurn();
                NextTurn();
            }
            pendingChar.OnTurnStart();
        }
        else
        {
            NextRound();
        }
    }

    public void StartTargeting(Ability ability)
    {
        if (ability.Equals(lastClickedAbility))
        {
            pendingCrit = !pendingCrit;
        } else
        {
            pendingCrit = false;
        }

        pendingAbility = ability;
        lastClickedAbility = ability;
        HighlightCharacters(GetTeam(TargetTeam()), true);
        if (pendingCrit)
        {
            abilityDescription.SetDescription(pendingAbility.CritDescription(pendingChar));
        }
        else
        {
            abilityDescription.SetDescription(pendingAbility.FullDescription(pendingChar));
        }

        click.selected = true;
    }

    public IEnumerator SelectTarget(Character target)
    {
        if (CheckTarget(target))
        {
            if (pendingChar.energy >= pendingAbility.energyCost)
            {
                if (!pendingCrit || pendingChar.currCrit >= pendingAbility.critCost)
                {
                    ClearActionText(characters);
                    List<Character> targets = GetTargets(target);
                    pendingChar.energy -= pendingAbility.energyCost;
                    energyDisplay.SetDescription(pendingChar.energy + " / " + pendingChar.stats.maxEnergy);
                    if (pendingCrit)
                    {
                        pendingChar.currCrit -= pendingAbility.critCost;
                        critDisplay.SetDescription(pendingChar.currCrit + "");
                    }
                    click.paused = true;
                    Ability saveAbility = pendingAbility;
                    StopTargeting();
                    yield return StartCoroutine(dm.ShowAbilityUse(pendingChar, saveAbility, targets));
                    yield return new WaitForSeconds(0.1f);
                    dm.SetActionDuration(targets, 400);
                    foreach (Character ch in targets)
                    {
                        saveAbility.Activate(pendingChar, ch, pendingCrit);
                    }
                    click.paused = false;
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
        StopTargeting();
    }

    public void StopTargeting()
    {
        HighlightCharacters(characters, false);
        abilityDescription.SetDescription("");

        pendingAbility = null;
        click.selected = false;
    }

    public List<Character> PreviewAbility(Character ch)
    {
        if(pendingChar != null && pendingAbility != null && CheckTarget(ch))
        {
            List<Character> affected = GetTargets(ch);
            return dm.PreviewAbility(pendingChar, pendingAbility, affected, pendingCrit);
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
