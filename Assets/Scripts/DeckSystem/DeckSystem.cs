using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckSystem : MonoBehaviour
{
    public List<CardData> deckList;
    public List<CardData> availableList;

    public void InitializeDeck(List<CardData> deck)
    {
        foreach (CardData card in deck)
        {
            deckList.Add(card);
        }
    }

    public void InitializeAvailable(List<CardData> available)
    {
        foreach (CardData card in available)
        {
            availableList.Add(card);
        }
    }

    public void HandleCardOnClick(RewardCard clickedCard)
    {
        GameObject clickedList = clickedCard.transform.parent.transform.parent.transform.parent.gameObject;
    }
}