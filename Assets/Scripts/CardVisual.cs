using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardVisual : MonoBehaviour
{
    [SerializeField] private Card card;
    [SerializeField] private TextMeshPro title;
    [SerializeField] private TextMeshPro cost;
    [SerializeField] private TextMeshPro description;

    private void Start()
    {
        title.SetText(card.title);
        cost.SetText(card.apCost.ToString());
        string effectText = "";
        for (int i = 0; i < card.effects.Length; i++)
        {
            CardEffect effect = card.effects[i];
            switch (effect.effectType)
            {
                case CardEffectType.Damage:
                {
                    effectText += $"Deal {effect.payload[0].ToString()} damage";
                    break;
                }
                case CardEffectType.Defend:
                {
                    effectText += $"Block {effect.payload[0].ToString()} damage";
                    break;
                }
                default: break;
            }
        }
        description.SetText(effectText);
    }
}
