using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = System.Random;

public class DeckManager
{
    private List<CardData> drawPile = new List<CardData>();
    private List<CardData> discardPile = new List<CardData>();

    public DeckManager(List<CardData> deck)
    {
        drawPile = Shuffle(deck);
    }

    public CardData DrawCard()
    {
        if (drawPile.Count == 0 && discardPile.Count == 0) return null;
        if (drawPile.Count == 0)
        {
            drawPile = Shuffle(discardPile);
            discardPile = new List<CardData>();
        }

        CardData nextCard = drawPile[0];
        drawPile.RemoveAt(0);
        return nextCard;
    }

    public void DiscardCard(CardData card)
    {
        discardPile.Add(card);
    }

    public List<CardData> Shuffle(List<CardData> cards)
    {
        Random r = new Random();
        int n = cards.Count;
        while (n > 1)
        {
            n--;
            int k = r.Next(n + 1);
            (cards[k], cards[n]) = (cards[n], cards[k]);
        }

        return cards;
    }
}