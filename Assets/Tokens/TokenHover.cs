using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class TokenHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    GameObject hoverBG;
    TMP_Text hoverText;
    Character owner;

    public string description;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hoverText = transform.Find("Info Text Background/Token Description").gameObject.GetComponent<TMP_Text>();
        hoverBG = transform.Find("Info Text Background").gameObject;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hoverText.text = description;
        hoverBG.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hoverBG.SetActive(false);
    }
}
