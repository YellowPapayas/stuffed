using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Healthbar : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Character character;
    public Transform charPos;

    public Vector3 offset;

    Image bar;
    GameObject backbar;

    GameObject text = null;
    public GameObject asset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bar = GetComponent<Image>();
        backbar = transform.parent.Find("Backbar").gameObject;
        character = transform.parent.parent.GetComponent<Character>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = charPos.position + offset;
        backbar.transform.position = transform.position + new Vector3(0, 0, 1);

    }

    public void UpdateView()
    {
        bar.fillAmount = ((float)character.health) / character.stats.maxHealth;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        text = Instantiate(asset);
        text.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
        text.transform.SetParent(transform.parent);
        text.GetComponent<TMP_Text>().text = character.health + "/" + character.stats.maxHealth;
        text.transform.position = transform.position + text.GetComponent<HoverText>().offset;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Destroy(text.gameObject);
    }
}
