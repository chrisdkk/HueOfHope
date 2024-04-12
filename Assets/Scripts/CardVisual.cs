using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardVisual : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI cost;
    [SerializeField] private TextMeshProUGUI description;
    public CardData cardData;

    private void Start()
    {
        UpdateCardVisual();
    }

    public void UpdateCardVisual()
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
