using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardListUIController : MonoBehaviour
{
    [SerializeField] private GameObject cardListCanvas;
    [SerializeField] private GameObject cardGrid;
    [SerializeField] private CardPool cardPool;

    private List<GameObject> cardsInList;

    public void OpenDeck()
    {
        Open(GameStateManager.Instance.deck);
    }

    public void OpenDrawPile()
    {
        Open(BattleManager.Instance.DeckManager.DrawPile);
    }

    public void OpenDiscardPile()
    {
        Open(BattleManager.Instance.DeckManager.DiscardPile);
    }
    
    private void Open(List<CardData> cards)
    {
        cards.Sort((x,y) => String.Compare(x.cardName, y.cardName, StringComparison.Ordinal));
        cards.Sort((x, y) => ((int)x.cardType).CompareTo((int)y.cardType));

        cardListCanvas.SetActive(true);
        
        cardPool.Populate(cards);
    }

    public void Close()
    {
        cardPool.Clear();
        cardListCanvas.SetActive(false);
    }
}
