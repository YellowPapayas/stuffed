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

        UpdateLines();
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
