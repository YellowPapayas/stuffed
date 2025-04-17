using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DescriptionText : MonoBehaviour
{
    TMP_Text desc;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        desc = GetComponent<TMP_Text>();
    }

    public void SetDescription(string text)
    {
        desc.text = text;
    }
}
