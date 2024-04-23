using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardVisual : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private Image[] apCostImages;
    public CardData cardData;

    private void Start()
    {
        UpdateCardVisual();
    }

    public void UpdateCardVisual()
    {
        title.SetText(cardData.cardName);
        description.SetText(cardData.description);
        for (int i = 0; i < cardData.apCost; i++)
        {
            apCostImages[i].gameObject.SetActive(true);
        }
    }
}
