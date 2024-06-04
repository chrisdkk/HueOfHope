using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RewardCardListUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cardCount;
    [SerializeField] private RewardCardPool cardPool;

    private List<GameObject> cardsInList;

    public void OpenDeck(RewardCard.NonBattleCardClickedEventHandler onClickFunction)
    {
        Open(GameInitializer.Instance.deck, onClickFunction);
    }

    private void Open(List<CardData> cards, RewardCard.NonBattleCardClickedEventHandler onClickFunction)
    {
        cards.Sort((x,y) => String.Compare(x.cardName, y.cardName, StringComparison.Ordinal));
        cards.Sort((x, y) => ((int)x.cardType).CompareTo((int)y.cardType));
        
        cardCount.text = cards.Count.ToString();
        cardPool.Populate(cards, onClickFunction);
    }

    public void Close()
    {
        cardPool.Clear();
    }
}
