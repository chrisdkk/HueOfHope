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
        description.SetText(cardData.description);
    }
}
