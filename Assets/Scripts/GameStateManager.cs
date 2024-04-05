using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{

    public static GameStateManager Instance;

    public int maxPlayerHealth;
    public int CurrentPlayerHealth { get; set; }
    public int MaxActionPoints { get; set; }
    public List<Card> Deck{ get; private set; }

    // Add this specific card to the deck
    public void AddCardToDeck(Card c)
    {
        Deck.Add(c);
    }
    
    // Remove this specific card from the deck and return if it succeeded
    public bool RemoveCardFromDeck(Card c)
    {
        return Deck.Remove(c);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        // Initialize 1 GameStateManager per game
        if (Instance == null)
        {
            Instance = this;
            maxPlayerHealth = 30;
            MaxActionPoints = 3;
            CurrentPlayerHealth = maxPlayerHealth;
            Deck = new List<Card>();
            //Add standard deck
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartStageBattle()
    {
        BattleManager bsm = new BattleManager();
        List<Enemy> enemies = new List<Enemy>();
        bsm.Initialize(Deck, enemies);
    }
}
