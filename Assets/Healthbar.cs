using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

public class Healthbar : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    GameObject healthText;
    Character character;
    Image slowbar;
    Image bar;

    int lastHealthCheck = 0;

    Coroutine damageRoutine = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        healthText = transform.parent.Find("Health Text").gameObject;
        healthText.SetActive(false);

        character = transform.parent.parent.parent.gameObject.GetComponent<Character>();
        lastHealthCheck = character.stats.maxHealth;

        slowbar = transform.parent.Find("Slowbar").gameObject.GetComponent<Image>();

        bar = GetComponent<Image>();
    }

    public void UpdateView()
    {
        Image firstbar;
        Image lastbar;
        if (lastHealthCheck < character.health)
        {
            firstbar = slowbar;
            lastbar = bar;
        } else
        {
            firstbar = bar;
            lastbar = slowbar;
        }
        float targetFill = ((float)character.health) / character.stats.maxHealth;
        firstbar.fillAmount = targetFill;

        if (damageRoutine != null) {
            StopCoroutine(damageRoutine);
        }

        damageRoutine = StartCoroutine(AnimateSlowbar(targetFill, lastbar));
        lastHealthCheck = character.health;
    }

    IEnumerator AnimateSlowbar(float targetFill, Image backbar)
    {
        yield return new WaitForSeconds(0.25f);

        float startFill = backbar.fillAmount;
        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            backbar.fillAmount = Mathf.Lerp(startFill, targetFill, elapsed / duration);
            yield return null;
        }

        backbar.fillAmount = targetFill;

        if (character.health <= 0)
        {
            yield return new WaitForSeconds(0.5f);
            character.OnDeath();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        healthText.GetComponent<TMP_Text>().text = character.health + " / " + character.stats.maxHealth;
        healthText.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        healthText.SetActive(false);
    }
}
