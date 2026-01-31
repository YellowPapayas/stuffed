using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class DisplayManager : MonoBehaviour
{
    GameObject overlayCanvas;
    TMP_Text abilityText;
    TMP_Text characterText;

    DescriptionText energyDisplay;
    DescriptionText critDisplay;

    BattleManager bm;
    ClickHandle click;

    List<Character> previewCharacters = new List<Character>();
    List<GameObject> playerUI = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        overlayCanvas = GameObject.Find("Main Camera").transform.Find("Overlay").gameObject;
        abilityText = overlayCanvas.transform.Find("Ability Name").gameObject.GetComponent<TMP_Text>();
        characterText = overlayCanvas.transform.Find("Character Name").gameObject.GetComponent<TMP_Text>();
        overlayCanvas.SetActive(false);

        energyDisplay = GameObject.Find("Energy Display").GetComponent<DescriptionText>();
        energyDisplay.Setup();
        critDisplay = GameObject.Find("Crit Display").GetComponent<DescriptionText>();
        critDisplay.Setup();

        bm = gameObject.GetComponent<BattleManager>();

        if (playerUI.Count <= 0)
        {
            playerUI.Add(GameObject.Find("End Turn"));
            playerUI.Add(GameObject.Find("Crit Display"));
            playerUI.Add(GameObject.Find("Energy Display"));
        }
    }

    public void Setup()
    {
        energyDisplay = GameObject.Find("Energy Display").GetComponent<DescriptionText>();
        energyDisplay.Setup();
        critDisplay = GameObject.Find("Crit Display").GetComponent<DescriptionText>();
        critDisplay.Setup();
    }

    public void UpdatePlayerUI(Character character)
    {
        energyDisplay.SetDescription(character.energy + " / " + character.stats.maxEnergy);
        critDisplay.SetDescription(character.currCrit + "");
    }

    public IEnumerator ShowAbilityUse(Character user, Ability pendingAbility, List<Character> targets)
    {
        characterText.text = user.stats.characterName + " uses:";
        abilityText.text = pendingAbility.abilityName + (bm.pendingCrit ? "\n<color=#66CCCC>CRITICAL!</color>" : "");
        overlayCanvas.SetActive(true);

        yield return new WaitForSeconds(1.3f);

        overlayCanvas.SetActive(false);

    }

    public List<Character> PreviewAbility(Character user, Ability pendingAbility, List<Character> toPreview, bool pendingCrit)
    {
        foreach (Character tar in toPreview)
        {
            pendingAbility.ActionText(user, tar, pendingCrit);
        }
        toPreview.Add(user);
        previewCharacters = toPreview;
        return toPreview;
    }

    public void SetActionDuration(int duration)
    {
        foreach (Character c in previewCharacters)
        {
            c.gameObject.GetComponent<CharacterUI>().ActionDuration(duration);
        }
    }

    public void ClearPlayerUI()
    {
        GameObject.Find("AbilityBar").GetComponent<AbilityBar>().ClearDisplay();
        if(playerUI.Count <= 0)
        {
            playerUI.Add(GameObject.Find("End Turn"));
            playerUI.Add(GameObject.Find("Crit Display"));
            playerUI.Add(GameObject.Find("Energy Display"));
        }
        foreach (GameObject go in playerUI)
        {
            go.SetActive(false);
        }
    }

    public void ShowPlayerUI()
    {
        foreach (GameObject go in playerUI)
        {
            go.SetActive(true);
        }
    }
}
