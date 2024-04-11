using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public struct PlayerGlobalStats
{
    public int maxHealth;
    public int currentHealth;
    public int maxActionPoints;
}

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;
    public BattleManager BattleManager { get; private set; }

    [SerializeField] private BattleManager battleManagerPrefab;

    public int CurrentPlayerHealth { get; set; }
    public int maxPlayerHealth;
    public int MaxActionPoints { get; set; }
    public List<CardData> deck;
    public CardData[] allAvailableCards;
    private List<Enemy> Enemies = new List<Enemy>();

    [SerializeField] public Enemy prototypeEnemy;

    // DECK SYSTEM
    [SerializeField] private DeckSystem deckSystem;
    // DECK SYSTEM

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
            deck = new List<CardData>();

            Enemies.Add(prototypeEnemy);

            allAvailableCards = Resources.LoadAll<CardData>("Cards/");

            //Add standard deck for prototype
            for (int i = 0; i < 15; i++)
            {
                deck.Add(allAvailableCards[i % 2]);
            }

            // Debug.Log(deck.Count);
            StartBattle();
        }
    }

    private void StartBattle()
    {
        // BattleManager = Instantiate(battleManagerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        // BattleManager.Initialize(deck, Enemies);
        
        // DECK SYSTEM
        deckSystem.InitializeDeck(deck);
        // DECK SYSTEM
    }

    // Add this specific card to the deck
    public void AddCardToDeck(CardData card)
    {
        deck.Add(card);
    }

    // Remove this specific card from the deck and return if it succeeded
    public bool RemoveCardFromDeck(CardData card)
    {
        return deck.Remove(card);
    }
}