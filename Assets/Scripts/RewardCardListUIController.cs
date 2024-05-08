using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardCardListUIController : MonoBehaviour
{
    [SerializeField] private GameObject cardListCanvas;
    [SerializeField] private GameObject cardGrid;
    [SerializeField] private RewardCardPool cardPool;

    private List<GameObject> cardsInList;

    public void OpenDeck(RewardCard.NonBattleCardClickedEventHandler onClickFunction)
    {
        Open(GameStateManager.Instance.deck, onClickFunction);
    }

    private void Open(List<CardData> cards, RewardCard.NonBattleCardClickedEventHandler onClickFunction)
    {
        cards.Sort((x,y) => String.Compare(x.cardName, y.cardName, StringComparison.Ordinal));
        cards.Sort((x, y) => ((int)x.cardType).CompareTo((int)y.cardType));
        
        
        cardPool.Populate(cards, onClickFunction);
    }

    public void Close()
    {
        cardPool.Clear();
    }
}
