using UnityEngine;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour
{
    [SerializeField] private Healthbar healthbar;
    [SerializeField] private StatusBar statusBar;
    [SerializeField] private TemporaryText actionText;
    [SerializeField] private GameObject highlight;

    Color highlightColor = new Color(228 / 255f, 1f, 0f, 120 / 255f);
    Color turnColor = new Color(120 / 255f, 180 / 255f, 120 / 255f, 90 / 255f);

    public void UpdateHealth()
    {
        healthbar?.UpdateView();
    }

    public void UpdateStatuses()
    {
        statusBar.UpdateView();
    }

    public void ShowActionText(string text, Color? color = null)
    {
        if (actionText == null) return;
        actionText.DisplayText(text, 400);
    }

    public void SetHighlight(bool isTurn, bool show)
    {
        Image light = highlight.GetComponent<Image>();
        if (isTurn)
        {
            highlight.SetActive(true);
            if (show)
            {
                light.color = highlightColor;
            }
            else
            {
                light.color = turnColor;
            }
        }
        else
        {
            light.color = highlightColor;
            highlight.SetActive(show);
        }
    }

    public void DisplayActionPerm(string perm)
    {
        actionText.PermText(perm);
    }

    public void ActionDuration(int duration)
    {
        actionText.SetDuration(duration);
    }

    public void ActionOff()
    {
        actionText.PermOff();
    }
}
