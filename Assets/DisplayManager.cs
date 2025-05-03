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

    BattleManager bm;
    ClickHandle click;

    List<GameObject> playerUI = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        overlayCanvas = GameObject.Find("Main Camera").transform.Find("Overlay").gameObject;
        abilityText = overlayCanvas.transform.Find("Ability Name").gameObject.GetComponent<TMP_Text>();
        characterText = overlayCanvas.transform.Find("Character Name").gameObject.GetComponent<TMP_Text>();
        overlayCanvas.SetActive(false);

        bm = gameObject.GetComponent<BattleManager>();

        if (playerUI.Count <= 0)
        {
            playerUI.Add(GameObject.Find("End Turn"));
            playerUI.Add(GameObject.Find("Crit Display"));
            playerUI.Add(GameObject.Find("Energy Display"));
        }
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
        //toPreview.Add(user);
        return toPreview;
    }

    public void SetActionDuration(List<Character> toSet, int duration)
    {
        foreach (Character c in toSet)
        {
            c.ActionDuration(duration);
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
