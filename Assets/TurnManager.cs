using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class TurnManager
{
    LinkedList<Character> turnQueue = new LinkedList<Character>();
    List<Character> characters;
    Character currentChar;

    BattleManager bm;
    TurnOrderDisplay turnOrderDisplay;

    public TurnManager(List<Character> allCharacters, TurnOrderDisplay display, BattleManager manager)
    {
        characters = allCharacters;
        turnOrderDisplay = display;
        display.Setup(characters);
        bm = manager;

        QueueTurns();
    }

    void QueueTurns()
    {
        turnQueue.Clear();
        var ordered = characters.OrderByDescending(c => c.stats.initiative);
        foreach (var ch in ordered)
            turnQueue.AddLast(ch);

        turnOrderDisplay.AddAllIcons(turnQueue);
    }

    public void NextTurn()
    {
        if (currentChar != null)
        {
            currentChar.OnTurnEnd();
            foreach (var ch in characters)
                ch.OnGeneralTurnEnd();

            currentChar = null;
            turnOrderDisplay.RemoveTopTurn();
        }

        if (turnQueue.Count > 0)
        {
            currentChar = turnQueue.First.Value;
            turnQueue.RemoveFirst();
            bm.SetPendingChar(currentChar);
            currentChar.OnTurnStart();
        }
        else
        {
            QueueTurns();
            NextTurn();
        }
    }

    public void OnCharacterDeath(Character ch)
    {

        characters.Remove(ch);

        var node = turnQueue.Find(ch);
        if (node != null)
        {
            turnQueue.Remove(node);
            turnOrderDisplay.RemoveIcon(ch);
        }

        foreach (Character character in characters)
            character.OnOtherCharacterDeath(ch);

        CheckBattleEnd();
    }

    void CheckBattleEnd()
    {
        var leftAlive = characters.Any(c => c.teamSide == true);
        var rightAlive = characters.Any(c => c.teamSide == false);

        if (!leftAlive || !rightAlive)
        {
            Debug.Log("Battle Ended!");
            // Could notify BattleManager to show win/lose UI here
        }
    }

    public Character CurrentCharacter => currentChar;
}
