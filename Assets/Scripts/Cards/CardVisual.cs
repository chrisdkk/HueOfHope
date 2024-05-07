using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CardVisual : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI cost;
    [SerializeField] private RawImage cardImage;
    public CardData CardData { get; private set; }

    public void LoadCardData(CardData newData)
    {
        CardData = newData;
        title.SetText(CardData.cardName);
        description.SetText(CardData.description);
        cost.SetText(CardData.apCost.ToString());
        cardImage.texture = CardData.cardImage;
    }
}
