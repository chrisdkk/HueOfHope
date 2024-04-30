using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = System.Random;

public class DeckManager
{
    public List<CardData> DrawPile { get; private set; }
    public List<CardData> DiscardPile { get; private set; }

    public DeckManager(List<CardData> deck)
    {
        DrawPile = Shuffle(new List<CardData>(deck));
        DiscardPile = new List<CardData>();
    }

    public CardData DrawCard()
    {
        if (DrawPile.Count == 0 && DiscardPile.Count == 0) return null;
        if (DrawPile.Count == 0)
        {
            DrawPile = Shuffle(DiscardPile);
            DiscardPile = new List<CardData>();
        }

        CardData nextCard = DrawPile[0];
        DrawPile.RemoveAt(0);
        return nextCard;
    }

    public void DiscardCard(CardData card)
    {
        DiscardPile.Add(card);
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
