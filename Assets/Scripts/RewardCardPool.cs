using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardCardPool : BaseCardPool
{
    public void Populate(List<CardData> cards, RewardCard.NonBattleCardClickedEventHandler onClickFunction)
    {
        if (cards.Count > pool.Count)
        {
            AddStep();
        }

        for (int i = 0; i < cards.Count; i++)
        {
            GameObject card = pool.Dequeue();
            card.SetActive(true);
            card.GetComponent<CardVisual>().LoadCardData(cards[i]);
            RewardCard rewardCard = card.GetComponent<RewardCard>();
            rewardCard.OnClick += onClickFunction;
            rewardCard.increasedScale = 1.1f;
        }
    }
}