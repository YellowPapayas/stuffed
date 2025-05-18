using UnityEngine;
using UnityEngine.UI;

public class AbilityBar : MonoBehaviour
{
    public GameObject buttonPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void UpdateView()
    {
        foreach (Transform child in transform)
        {
            AbilityButton buttonUI = child.gameObject.GetComponent<AbilityButton>();
            buttonUI.UpdateView();
        }
    }

    public void DisplayAbilities(Character user)
    {
        ClearDisplay();

        foreach(Ability ab in user.abilities)
        {
            GameObject button = Instantiate(buttonPrefab, transform);

            AbilityButton buttonUI = button.GetComponent<AbilityButton>();
            buttonUI.Setup(ab, user);
        }
    }

    public void ClearDisplay()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
