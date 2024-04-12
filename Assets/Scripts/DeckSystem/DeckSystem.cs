using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckSystem : MonoBehaviour
{
    public List<CardData> deckList;
    public List<CardData> availableList;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void InitializeDeck(List<CardData> deck)
    {
        foreach (CardData card in deck)
        {
            deckList.Add(card);
        }
    }

    public void InitializeAvailable(List<CardData> available)
    {
        foreach (CardData card in available)
        {
            availableList.Add(card);
        }
    }

    public void HandleCardOnClick(NonBattleCard clickedCard)
    {
        Debug.Log(clickedCard.data.name);
        
        // todo: make this more efficient
        GameObject clickedList = clickedCard.transform.parent.transform.parent.transform.parent.gameObject;

        if (clickedList.name == "AvailableCards")
        {
            availableList.Remove(clickedCard.data);
            Debug.Log(clickedCard.data.name);
            
            deckList.Add(clickedCard.data);
        }
        
        if (clickedList.name == "CurrentDeck")
        {
            deckList.Remove(clickedCard.data);
            Debug.Log(clickedCard.data.name);
            
            availableList.Add(clickedCard.data);
        }
    }
}