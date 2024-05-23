using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Effect", menuName = "ScriptableObjects/Effect", order = 1)]
public class EffectData : ScriptableObject
{
    public string title;
    public string text;
    public string effectDescription;

    // Insert payload into the text and determine to whom the effect is applied
    public string GetText(int payload, bool damageUpdated, int multiplier, CardEffectTarget cardEffectTarget, bool casterIsPlayer)
    {
        // Add payload
        string currText = damageUpdated
            ? text.Replace("[NUMBER]", "<color=yellow>" + payload + "</color>")
            : text.Replace("[NUMBER]", payload.ToString());

        // Change word according to target (player or enemy)
        if (text.Contains("["))
        {
            string target = text.Substring(text.IndexOf("["), text.IndexOf("]") + 1);
            string[] targetOptions = target.Replace("[", "").Replace("]", "").Split("/");

            currText = (cardEffectTarget == CardEffectTarget.Player && casterIsPlayer) || (cardEffectTarget != CardEffectTarget.Player && !casterIsPlayer)
                ? currText.Replace(target, targetOptions[0])
                : currText.Replace(target, targetOptions[1]);
        }

        currText = currText.Replace("{MULTI}", multiplier.ToString());

        // Add aoe text if necessary
        return cardEffectTarget == CardEffectTarget.MultipleEnemies ? currText + " to all enemies" : currText;
    }
}