using UnityEngine;
using TMPro;

public class TemporaryText : MonoBehaviour
{
    TMP_Text text;
    int frames = 0;
    bool perm = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if(frames > 0)
        {
            frames--;
        } else if (!perm)
        {
            text.gameObject.SetActive(false);
        }
    }

    public void PermText(string txt)
    {
        text.gameObject.SetActive(true);
        text.text = txt;
        perm = true;
    }

    public void AddText(string txt)
    {
        if (text.text != txt)
        {
            if (frames > 0 || perm)
            {
                text.text += "\n" + txt;
            }
            else
            {
                PermText(txt);
            }
        }
    }

    public void PermOff()
    {
        perm = false;
    }

    public void DisplayText(string txt, int duration)
    {
        PermOff();
        text.gameObject.SetActive(true);
        frames = duration;
        text.text = txt;
    }

    public void SetDuration(int duration)
    {
        text.gameObject.SetActive(true);
        frames = duration;
        perm = false;
    }
}
