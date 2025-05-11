using UnityEngine;
using System.Collections.Generic;

public class LineManager : MonoBehaviour
{
    GameObject leftFront, leftMid, leftBack, rightFront, rightMid, rightBack;
    public List<GameObject> lines;
    public float spacing;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (leftFront == null)
        {
            Setup();
        }
        UpdateLines();
    }

    void Setup()
    {
        leftFront = GameObject.Find("Left Front Line");
        leftMid = GameObject.Find("Left Mid Line");
        leftBack = GameObject.Find("Left Back Line");
        rightFront = GameObject.Find("Right Front Line");
        rightMid = GameObject.Find("Right Mid Line");
        rightBack = GameObject.Find("Right Back Line");

        lines = new List<GameObject>();
        lines.Add(leftFront);
        lines.Add(leftMid);
        lines.Add(leftBack);
        lines.Add(rightFront);
        lines.Add(rightMid);
        lines.Add(rightBack);
    }

    public void UpdateLines()
    {
        foreach (GameObject line in lines)
        {
            int childCount = line.transform.childCount;
            float num = 0;
            float startPoint = -(spacing / 2) * (childCount-1) + line.transform.position.y;
            foreach (Transform child in line.transform)
            {
                child.position = new Vector3(line.transform.position.x, startPoint + (spacing * num), line.transform.position.z);
                num += 1;

                Character childChar = child.gameObject.GetComponent<Character>();
                if (line == leftFront || line == rightFront)
                {
                    childChar.passiveMods.Add(new PassiveModifier(StatType.Accuracy, 10));
                    childChar.passiveMods.Add(new PassiveModifier(StatType.Dodge, -10));
                }
                if (line == leftBack || line == rightBack)
                {
                    childChar.passiveMods.Add(new PassiveModifier(StatType.Accuracy, -10));
                    childChar.passiveMods.Add(new PassiveModifier(StatType.Dodge, 10));
                }
            }
        }
    }

    public List<Character> InSameLine(Character ch)
    {
        Transform tr = ch.gameObject.transform.parent;
        List<Character> output = new List<Character>();

        foreach (Transform child in tr)
        {
            output.Add(child.gameObject.GetComponent<Character>());
        }
        return output;
    }

    public List<Character> OneFromEach(bool teamSide)
    {
        if(leftFront == null)
        {
            Setup();
        }
        List<Character> output = new List<Character>(3);
        if(teamSide)
        {
            if(leftFront.transform.childCount > 0)
            {
                output.Add(leftFront.transform.GetChild(0).gameObject.GetComponent<Character>());
            }
            if (leftMid.transform.childCount > 0)
            {
                output.Add(leftMid.transform.GetChild(0).gameObject.GetComponent<Character>());
            }
            if (leftBack.transform.childCount > 0)
            {
                output.Add(leftBack.transform.GetChild(0).gameObject.GetComponent<Character>());
            }
        } else
        {
            if (rightFront.transform.childCount > 0)
            {
                output.Add(rightFront.transform.GetChild(0).gameObject.GetComponent<Character>());
            }
            if (rightMid.transform.childCount > 0)
            {
                output.Add(rightMid.transform.GetChild(0).gameObject.GetComponent<Character>());
            }
            if (rightBack.transform.childCount > 0)
            {
                output.Add(rightBack.transform.GetChild(0).gameObject.GetComponent<Character>());
            }
        }
        return output;
    }
}
