using System.Collections.Generic;
using UnityEngine;

public class CardPool : BaseCardPool
{
    public void Populate(List<CardData> cards)
    {
        while (cards.Count > pool.Count)
        {
            AddStep();
        }

        for (int i = 0; i < cards.Count; i++)
        {
            GameObject card = pool.Dequeue();
            card.SetActive(true);
            card.GetComponent<CardVisual>().LoadCardData(cards[i]);
        }
    }
}