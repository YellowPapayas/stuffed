using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TurnOrderDisplay : MonoBehaviour
{
    public GameObject turnIcon;
    Dictionary<int, GameObject> icons = new Dictionary<int, GameObject>();
    LinkedList<GameObject> iconOrder = new LinkedList<GameObject>();

    public void Setup(List<Character> list)
    {
        foreach (Character ch in list)
        {
            GameObject icon = Instantiate(turnIcon);
            icon.transform.SetParent(transform.parent);
            icon.GetComponent<Image>().sprite = ch.gameObject.GetComponent<SpriteRenderer>().sprite;
            icons.Add(ch.gameObject.GetInstanceID(), icon);
            icon.SetActive(false);
        }
    }

    public void AddAllIcons(LinkedList<Character> order)
    {
        foreach (Character ch in order)
        {
            GameObject ic;
            icons.TryGetValue(ch.gameObject.GetInstanceID(), out ic);
            ic.transform.SetParent(transform);
            ic.SetActive(true);

            iconOrder.AddLast(ic);
        }
    }

    public void RemoveTopTurn()
    {
        if (iconOrder.Count > 0)
        {
            GameObject ic = iconOrder.First.Value;
            iconOrder.RemoveFirst();
            ic.transform.SetParent(transform.parent);
            ic.SetActive(false);
        }
    }

    public void RemoveIcon(Character ch)
    {
        GameObject ic;
        icons.TryGetValue(ch.gameObject.GetInstanceID(), out ic);

        var toRemove = iconOrder.Find(ic);
        if(toRemove != null)
        {
            iconOrder.Remove(toRemove);
            icons.Remove(ch.gameObject.GetInstanceID());
            Destroy(ic);
        }
    }
}
