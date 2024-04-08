using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardVisual : MonoBehaviour
{
    [SerializeField] private TextMeshPro title;
    [SerializeField] private TextMeshPro cost;
    [SerializeField] private TextMeshPro description;

    [SerializeField] private float selectedOffset;
    
    public void Initialize(CardData cardData)
    {
        title.SetText(cardData.cardName);
        cost.SetText(cardData.apCost.ToString());
        string effectText = "";
        foreach (CardEffect effect in cardData.effects)
        {
            switch (effect.effectType)
            {
                case CardEffectType.Damage:
                    effectText += $"Deal {effect.payload[0]} damage";
                    break;
                case CardEffectType.Defend:
                    effectText += $"Block {effect.payload[0]} damage";
                    break;
                default: break;
            }
        }
        description.SetText(effectText);
    }
}
