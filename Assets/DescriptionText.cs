using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DescriptionText : MonoBehaviour
{
    TMP_Text desc;

    public void Setup()
    {
        desc = GetComponent<TMP_Text>();
    }

    public void SetDescription(string text)
    {
        desc.text = text;
    }
}
