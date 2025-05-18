using UnityEngine;
using System.Collections.Generic;

public class TokenBar : MonoBehaviour
{
    public GameObject icon;
    List<GameObject> tokenIcons = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddToken(TokenObject tokenObject)
    {
        GameObject token = Instantiate(icon, transform);
        tokenObject.SetupIcon(token.GetComponent<TokenHover>());
        tokenIcons.Add(token);
    }

    public void RemoveToken(int pos)
    {
        Destroy(tokenIcons[pos]);
        tokenIcons.RemoveAt(pos);
    }
}
