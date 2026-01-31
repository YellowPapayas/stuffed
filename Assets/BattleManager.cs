using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class BattleManager : MonoBehaviour
{
    Character pendingChar;
    public Ability pendingAbility;

    [SerializeField] ClickHandle click;
    [SerializeField] LineManager lm;
    [SerializeField] DisplayManager dm;
    [SerializeField] DescriptionText abilityDescription;
    [SerializeField] TemporaryText warning;
    [SerializeField] TurnOrderDisplay turnOrderDisplay;

    TurnManager turnManager;

    List<Character> characters;
    List<Character> leftSide;
    List<Character> rightSide;
    Queue<Character> turnQueue;

    public bool pendingCrit = false;
    public Ability lastClickedAbility = null;

    public int dodgeCap;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        abilityDescription.Setup();
        dm.Setup();

        characters = new List<Character>();
        leftSide = characters.FindAll(c => c.teamSide);
        rightSide = characters.FindAll(c => !c.teamSide);
        AddCharacters();

        turnManager = new TurnManager(characters, turnOrderDisplay, this);
        turnManager.NextTurn();
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
            c.gameObject.GetComponent<CharacterUI>().ActionOff();
        }
    }

    public void SetPendingChar(Character ch)
    {
        pendingChar = ch;
    }

    public void NextTurn()
    {
        turnManager.NextTurn();
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
        // Guard 1: wrong team / line
        if (!CheckTarget(target))
            yield break;                                 // nothing to do; silently ignore

        // Guard 2: not enough energy
        if (pendingChar.energy < pendingAbility.energyCost)
        {
            SelectFail("Not enough energy!");
            yield break;
        }

        // Guard 3: not enough crit (only when spending crit)
        if (pendingCrit && pendingChar.currCrit < pendingAbility.critCost)
        {
            SelectFail("Not enough crit!");
            yield break;
        }

        ClearActionText(characters);
        List<Character> targets = GetTargets(target);
        pendingChar.energy -= pendingAbility.energyCost;
        if (pendingCrit)
        {
            pendingChar.currCrit -= pendingAbility.critCost;
        }
        dm.UpdatePlayerUI(pendingChar);
        click.paused = true;
        Ability saveAbility = pendingAbility;
        StopTargeting();
        yield return StartCoroutine(dm.ShowAbilityUse(pendingChar, saveAbility, targets));
        yield return new WaitForSeconds(0.1f);
        dm.SetActionDuration(800);
        saveAbility.Activate(pendingChar, targets, pendingCrit);
        GameObject.Find("AbilityBar").GetComponent<AbilityBar>().UpdateView();
        click.paused = false;
    }

    void SelectFail(string message)
    {
        warning.DisplayText($"<color=#AA0000>{message}</color>", 750);
        ClearActionText(characters);
        StopTargeting();
    }

    public void StopTargeting()
    {
        HighlightCharacters(characters, false);
        abilityDescription.SetDescription("");

        pendingAbility = null;
        click.selected = false;
    }

    void OnCharacterDeath(Character ch)
    {
        characters.Remove(ch);
        leftSide.Remove(ch);
        rightSide.Remove(ch);

        turnManager.OnCharacterDeath(ch);
    }

    public List<Character> PreviewAbility(Character ch)
    {
        if(pendingChar != null && pendingAbility != null && CheckTarget(ch))
        {
            List<Character> affected = new List<Character>();
            affected.AddRange(GetTargets(ch));
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

    void OnEnable()
    {
        Character.CharacterDied += OnCharacterDeath;
    }

    void OnDisable()
    {
        Character.CharacterDied -= OnCharacterDeath;
    }
}
