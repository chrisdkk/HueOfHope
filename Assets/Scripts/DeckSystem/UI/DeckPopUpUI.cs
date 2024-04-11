using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckPopUpUI : MonoBehaviour
{
    [SerializeField] private DeckSystem deckSystem;

    [SerializeField] private GameObject popUpPrefab;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Camera camera;

    private bool isDeckWindowCreated = false;
    private bool canOpenPopUp = false;

    private GameObject DeckWindowInstance;

    private void Start()
    {
        // instantiate only one pop up
        if (!isDeckWindowCreated)
        {
            DeckWindowInstance = Instantiate(popUpPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            isDeckWindowCreated = true;
            canOpenPopUp = true;
        }
    }

    public void OnButtonClick()
    {
        // this might re-instantiate its child prefabs (cardInstance), do we want that?
        if (canOpenPopUp)
        {
            DeckWindowInstance.SetActive(true);
            canOpenPopUp = false;
        }
        else
        {
            DeckWindowInstance.SetActive(false);
            canOpenPopUp = true;
        }

        float cardPosX = 100;
        float cardPosY = -100;

        // necessary?
        // DeckWindowInstance.GetComponentInChildren<Canvas>().planeDistance = 3;

        // make deck/map scene camera "see" the card prefab instances WHY?
        DeckWindowInstance.transform.GetComponentInChildren<Canvas>().worldCamera = camera;
        
        // get specific child (i know this is weird)
        Transform currentDeckPanel = DeckWindowInstance.transform.GetChild(0).GetChild(1).GetChild(0).transform;
        Transform availableCardsPanel = DeckWindowInstance.transform.GetChild(0).GetChild(0).GetChild(0).transform;

        foreach (CardData card in deckSystem.deckList)
        {
            cardPrefab.GetComponentInChildren<Card>().data = card;

            GameObject cardInstance = Instantiate(cardPrefab, currentDeckPanel, false);

            cardInstance.transform.localScale = new Vector3(50, 50, 50);
            cardInstance.transform.localPosition = new Vector3(cardPosX, cardPosY, -1f);

            cardPosX += 100;

            if (cardPosX >= 800)
            {
                cardPosX = 100;
                cardPosY -= 150;
            }
        }

        // // Adjust content height of scroll view
        // RectTransform rectTransform = currentDeckPanel.GetComponent<RectTransform>();
        // cardPosY = (cardPosY - 100) * -1;
        // rectTransform.sizeDelta = new Vector2(rectTransform.rect.size.x, cardPosY);
    }
}