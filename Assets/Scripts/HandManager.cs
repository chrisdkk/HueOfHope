using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class HandManager : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform handTransform;
    [SerializeField] private float horizontalSpacing;
    [SerializeField] private int maxHandSize;
    private DeckManager deck;
    private Stats stats;
    private List<GameObject> cardsInHand = new List<GameObject>();

    public void Initialize(DeckManager deckManager)
    {
        deck = deckManager;
    }

    public void DrawHand()
    {
        for (int i = 0; i < maxHandSize; i++)
        {
            AddCardToHand(deck.DrawCard());
        }
    }

    public void DiscardHand()
    {
        for (int i = 0; i < cardsInHand.Count; i++)
        {
            deck.DiscardCard(cardsInHand[i].GetComponent<CardVisual>().cardData);
            Destroy(cardsInHand[i]);
        }
        cardsInHand = new List<GameObject>();
    }

    public void AddCardToHand(CardData cardData)
    {
        if (cardData == null) return;
        GameObject newCard = Instantiate(cardPrefab, handTransform.position, Quaternion.identity, handTransform);
        cardsInHand.Add(newCard);

        // Set card visuals
        CardVisual cardVisual = newCard.GetComponent<CardVisual>();
        cardVisual.cardData = cardData;
        cardVisual.UpdateCardVisual();
        
        // Set card movement
        CardMovement cardMovement = newCard.GetComponent<CardMovement>();
        cardMovement.OnPlay += DiscardCard;
        cardMovement.OnDrag += CardDragHandler;
        cardMovement.OnDrop += CardDropHandler;
        cardMovement.Initialize(cardData);

        UpdateHandVisuals();
    }

    private void DiscardCard(GameObject card)
    {
        CardData cardData = card.GetComponent<CardVisual>().cardData;
        deck.DiscardCard(cardData);
        cardsInHand.Remove(card);
        Destroy(card);
        
        UpdateHandVisuals();
        UpdateHandSelectable();
    }

    private void UpdateHandVisuals()
    {
        int cardCount = cardsInHand.Count;
        float offset = cardPrefab.transform.localScale.x * this.horizontalSpacing;
        for (int i = 0; i < cardCount; i++)
        {
            GameObject card = cardsInHand[i];
            float x = offset * (i - 0.5f * (cardCount - 1));

            card.transform.position = new Vector3(x, handTransform.position.y, i);
        }
    }

    private void UpdateHandSelectable()
    {
        BattleManager battleManager = FindObjectOfType<BattleManager>();
        foreach (GameObject card in cardsInHand)
        {
            if (card.GetComponent<CardVisual>().cardData.apCost > battleManager.CurrentActionPoints)
            {
                card.GetComponent<CardMovement>().isSelectable = false;
            } 
        }
    }

    private void CardDragHandler(GameObject draggedCard)
    {
        foreach (GameObject card in cardsInHand)
        {
            if (card != draggedCard)
            {
                card.GetComponent<CardMovement>().isSelectable = false;
            }
        }
    }
    
    private void CardDropHandler(GameObject droppedCard)
    {
        foreach (GameObject card in cardsInHand)
        {
            if (card != droppedCard)
            {
                card.GetComponent<CardMovement>().isSelectable = true;
            }
        }
    }
    
}
