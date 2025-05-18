using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewAddTokenEffect", menuName = "CritEffects/AddTokenEffect")]
public class AddTokenEffect : AbilityEffect
{
    public List<TokenObject> tokenList;

    public override void AddEffect(List<AbilityAction> actions)
    {
        actions.Add(new AddTokenAction(tokenList));
    }

    public override string AddDescription()
    {
        string output = "Add ";
        for (int i = 0; i < tokenList.Count; i++)
        {
            TokenObject token = tokenList[i];
            output += $"a {token.tokenName} token";
            if(i < tokenList.Count - 1)
            {
                output += ", ";
            } else if(tokenList.Count > 1)
            {
                output += " and ";
            }
        }
        return output;
    }
}
