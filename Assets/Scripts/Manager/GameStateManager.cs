using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    [SerializeField] private BattleManager battleManager;
    [SerializeField] public Enemy prototypeEnemy;
    [SerializeField] private DeckSystem deckSystem;

    public int CurrentPlayerHealth { get; set; }
    public int maxPlayerHealth;
    public int MaxActionPoints { get; set; }
    public List<CardData> deck;
    public List<CardData> AllAvailableCards;
    private List<Enemy> Enemies = new List<Enemy>();

    public bool blueEnabled = false;
    public bool redEnabled = false;
    public bool greenEnabled = false;
    
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
            
            AllAvailableCards = Resources.LoadAll<CardData>("Cards/").ToList();

            //Add standard deck for prototype
            for (int i = 0; i < 5; i++)
            {
                deck.Add(AllAvailableCards.Find(cardData => cardData.cardName=="Cloak Block"));
                deck.Add(AllAvailableCards.Find(cardData => cardData.cardName=="Kick"));
            }
            deck.Add(AllAvailableCards.Find(cardData => cardData.cardName=="Penetrate"));
            deck.Add(AllAvailableCards.Find(cardData => cardData.cardName=="Barrier Strike"));
            deck.Add(AllAvailableCards.Find(cardData => cardData.cardName=="Blockbuster"));
            deck.Add(AllAvailableCards.Find(cardData => cardData.cardName=="Calm Drop"));
            deck.Add(AllAvailableCards.Find(cardData => cardData.cardName=="Flaming Insight"));
            deck.Add(AllAvailableCards.Find(cardData => cardData.cardName=="Fire Storm"));
            deck.Add(AllAvailableCards.Find(cardData => cardData.cardName=="Insightful Strike"));
            deck.Add(AllAvailableCards.Find(cardData => cardData.cardName=="Revelation"));
            deck.Add(AllAvailableCards.Find(cardData => cardData.cardName=="Shield Break"));
            
            // deck system
            deckSystem.InitializeDeck(deck);
            StartBattle();
        }
    }

    private void StartBattle()
    {
        battleManager.Initialize(deck, Enemies);
    }

}