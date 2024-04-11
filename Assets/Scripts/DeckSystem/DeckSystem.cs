using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckSystem : MonoBehaviour
{
    public List<CardData> deckList;

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
}