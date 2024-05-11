using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Effect", menuName = "ScriptableObjects/Effect", order = 1)]
public class EffectData : ScriptableObject
{
    public string title;
    public string text;
    public string effectDescription;

    // Insert payload into the text
    public string GetText(int payload)
    {
        return text.Replace("[NUMBER]", payload.ToString());
    }
}
