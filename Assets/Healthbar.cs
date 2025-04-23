using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Healthbar : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    GameObject healthText;
    Character character;
    Image bar;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        healthText = transform.parent.Find("Health Text").gameObject;
        healthText.SetActive(false);

        character = transform.parent.parent.parent.gameObject.GetComponent<Character>();

        bar = GetComponent<Image>();
    }

    public void UpdateView()
    {
        bar.fillAmount = ((float)character.health) / character.stats.maxHealth;
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
