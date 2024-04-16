using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DeckPopUpUI : MonoBehaviour, IDeselectHandler
{
    [SerializeField] private DeckSystem deckSystem;

    [SerializeField] private GameObject popUpPrefab;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Camera camera;

    private float cardScale = 80f;

    private bool isDeckWindowCreated = false;
    private bool canOpenPopUp = false;
    private bool hasListBeenFilled = false;

    private GameObject deckWindowInstance;

    private float deckCardPosX = 100;
    private float deckCardPosY = -100;

    private float availableCardPosX = 100;
    private float availableCardPosY = -100;

    private void Start()
    {
        // instantiate only one pop up if one is not already open
        if (!isDeckWindowCreated)
        {
            deckWindowInstance = Instantiate(popUpPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            isDeckWindowCreated = true;
            canOpenPopUp = true;
        }
    }

    // close deck ui pop up when clicking outside of it
    public void OnDeselect(BaseEventData eventData)
    {
        Debug.Log(eventData.selectedObject);
        deckWindowInstance.SetActive(false);
        canOpenPopUp = true;
    }

    public void OnButtonClick()
    {
        // this might re-instantiate its child prefabs (cardInstance), do we want that?
        if (canOpenPopUp)
        {
            deckWindowInstance.SetActive(true);
            canOpenPopUp = false;
        }
        else
        {
            deckWindowInstance.SetActive(false);
            canOpenPopUp = true;
        }

        // necessary?
        // deckWindowInstance.GetComponentInChildren<Canvas>().planeDistance = 3;

        // make deck/map scene camera "see" the card prefab instances WHY?
        deckWindowInstance.transform.GetComponentInChildren<Canvas>().worldCamera = camera;

        // get specific child, content in this case (this is weird)
        Transform currentDeckPanel =
            deckWindowInstance.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0).transform;
        // Transform availableCardsPanel =
        //     deckWindowInstance.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).transform;

        // deck cards tab (right)
        if (!hasListBeenFilled)
        {
            foreach (CardData card in deckSystem.deckList)
            {
                cardPrefab.GetComponentInChildren<NonBattleCard>().data = card;

                GameObject cardInstance = Instantiate(cardPrefab, currentDeckPanel, false);

                // cardInstance.GetComponent<NonBattleCard>().OnClick += deckSystem.HandleCardOnClick;

                cardInstance.transform.localScale = new Vector3(cardScale, cardScale, cardScale);
                cardInstance.transform.localPosition = new Vector3(deckCardPosX, deckCardPosY, -1f);

                deckCardPosX += 150;

                if (deckCardPosX >= 1700)
                {
                    deckCardPosX = 100;
                    deckCardPosY -= 200;
                }
            }

            hasListBeenFilled = true;
        }

        // available cards tab (left)
        // foreach (CardData card in deckSystem.availableList)
        // {
        //     cardPrefab.GetComponentInChildren<NonBattleCard>().data = card;
        //
        //     // if item gets "moved" from one list to another, remove prefab instance so visual reflects data
        //     GameObject cardInstance = Instantiate(cardPrefab, availableCardsPanel, false);
        //     
        //     cardInstance.GetComponent<NonBattleCard>().OnClick += deckSystem.HandleCardOnClick;
        //     
        //     cardInstance.transform.localScale = new Vector3(50, 50, 50);
        //     cardInstance.transform.localPosition = new Vector3(availableCardPosX, availableCardPosY, -1f);
        //
        //     availableCardPosX += 100;
        //
        //     if (availableCardPosX >= 700)
        //     {
        //         availableCardPosX = 100;
        //         availableCardPosY -= 150;
        //     }
        // }
    }
}