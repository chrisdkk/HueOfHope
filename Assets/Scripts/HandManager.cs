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
        UpdateHandVisuals();
    }

    public void AddCardToHand(CardData cardData)
    {
        if (cardData == null) return;
        GameObject newCard = Instantiate(cardPrefab, handTransform.position, Quaternion.identity, handTransform);
        cardsInHand.Add(newCard);

        CardVisual cardVisual = newCard.GetComponent<CardVisual>();
        cardVisual.cardData = cardData;
        cardVisual.UpdateCardVisual();

        UpdateHandVisuals();
    }

    private void DiscardCard(GameObject card)
    {
        CardData cardData = card.GetComponent<CardVisual>().cardData;
        deck.DiscardCard(cardData);
        cardsInHand.Remove(card);
        Destroy(card);
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
