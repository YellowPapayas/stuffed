using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class DisplayManager : MonoBehaviour
{
    GameObject overlayCanvas;
    TMP_Text abilityText;

    BattleManager bm;
    ClickHandle click;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        overlayCanvas = GameObject.Find("Main Camera").transform.Find("Overlay").gameObject;
        abilityText = overlayCanvas.transform.Find("Ability Name").gameObject.GetComponent<TMP_Text>();
        overlayCanvas.SetActive(false);

        bm = gameObject.GetComponent<BattleManager>();
    }

    public IEnumerator ShowAbilityUse(Ability pendingAbility, List<Character> targets)
    {
        abilityText.text = pendingAbility.abilityName;
        overlayCanvas.SetActive(true);

        yield return new WaitForSeconds(1.1f);

        overlayCanvas.SetActive(false);

    }

    public List<Character> PreviewAbility(Character user, Ability pendingAbility, List<Character> toPreview, bool pendingCrit)
    {
        foreach (Character tar in toPreview)
        {
            pendingAbility.ActionText(user, tar, pendingCrit);
        }
        toPreview.Add(user);
        return toPreview;
    }

    public void SetActionDuration(List<Character> toSet, int duration)
    {
        foreach (Character c in toSet)
        {
            c.ActionDuration(duration);
        }
    }
}
