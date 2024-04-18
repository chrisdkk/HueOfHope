using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TogglePopUpUI : MonoBehaviour, IDeselectHandler
{
    [SerializeField] private DeckSystem deckSystem;

    [SerializeField] private GameObject popUpPrefab;
    [SerializeField] private GameObject cardPrefab;
    
    [SerializeField] private new Camera camera; // new is required because of old unity code

    private bool isDeckWindowCreated = false;
    private bool canOpenPopUp = false;

    private GameObject deckWindowInstance;

    // Start is called before the first frame update
    void Start()
    {
        if (!isDeckWindowCreated)
        {
            deckWindowInstance = Instantiate(popUpPrefab, new Vector3(0, 0, 0), Quaternion.identity);

            deckWindowInstance.GetComponent<DeckPopUpUI>().deckSystem = deckSystem;

            isDeckWindowCreated = true;
            canOpenPopUp = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    // close deck ui pop up when clicking outside of it
    public void OnDeselect(BaseEventData eventData)
    {
        deckWindowInstance.SetActive(false);
        canOpenPopUp = true;
    }

    public void OnButtonClick()
    {
        // this might re-instantiate its child prefabs (cardInstance), do we want that? <- THIS MIGHT NOT BE TRUE ANYMORE, DON'T ASK ME
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
    }
}