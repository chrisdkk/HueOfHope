using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        cardMovement.OnPlay += PlayCard;
        cardMovement.Initialize(cardData);

        UpdateHandVisuals();
    }

    private void PlayCard(GameObject card, GameObject enemy)
    {
        CardData cardData = card.GetComponent<CardVisual>().cardData;

        bool cardPlayed;
        // Determine if card was offensive or defensive
        if (enemy != null)
        {
            cardPlayed = GameStateManager.Instance.BattleManager.PlayCard(GameStateManager.Instance.BattleManager.PlayerStats, true, cardData.apCost, cardData.effects, ref enemy.GetComponent<Enemy>().stats);
        }
        else
        {
            cardPlayed = GameStateManager.Instance.BattleManager.PlayCard(GameStateManager.Instance.BattleManager.PlayerStats, true, cardData.apCost, cardData.effects, ref GameStateManager.Instance.BattleManager.PlayerStats);
        }
        
        if (cardPlayed)
        {
            DiscardCard(card);
        }
    }
    
    private void DiscardCard(GameObject card)
    {
        CardData cardData = card.GetComponent<CardVisual>().cardData;
        deck.DiscardCard(cardData);
        cardsInHand.Remove(card);
        Destroy(card);
        
        UpdateHandVisuals();
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
}
