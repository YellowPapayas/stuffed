using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatsDisplay : MonoBehaviour
{
    public TMP_Text nameText, attack, armor, dodge, accuracy, crit, initiative, health, currDodge, energy, currCrit;
    bool isShown = false;
    ClickHandle click;

    public void DisplayStats(Character ch)
    {
        gameObject.SetActive(true);
        isShown = true;

        if (click == null)
        {
            click = GameObject.Find("ClickHandler").GetComponent<ClickHandle>();
        }

        click.paused = true;
        nameText.text = ch.stats.characterName;

        attack.text = "Attack: " + ch.stats.attack + ch.GetModString(StatType.Attack);
        armor.text = "Armor: " + ch.stats.armor + ch.GetModString(StatType.Armor);
        dodge.text = "Dodge: " + ch.stats.dodge + ch.GetModString(StatType.Dodge);
        accuracy.text = "Accuracy: " + ch.stats.accuracy + ch.GetPassiveString(StatType.Accuracy) + ch.GetModString(StatType.Accuracy);
        crit.text = "Crit: " + ch.stats.crit;
        initiative.text = "Initiative: " + ch.stats.initiative;

        health.text = "Health: " + ch.health + " / " + ch.stats.maxHealth;
        currDodge.text = "Dodge: " + ch.currDodge + ch.GetPassiveString(StatType.Dodge);
        energy.text = "Energy: " + ch.energy + " / " + ch.stats.maxEnergy;
        currCrit.text = "Crit: " + ch.currCrit;
    }

    // Update is called once per frame
    void Update()
    {
        if(isShown)
        {
            if(Input.GetMouseButtonDown(0))
            {
                gameObject.SetActive(false);
                isShown=false;
                click.paused = false;
            }
        }
    }
}
