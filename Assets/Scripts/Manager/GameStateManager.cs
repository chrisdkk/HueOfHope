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
    public int BurnTickDamage { get; set; }
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
            BurnTickDamage = 4;
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
            deck.AddRange(AllAvailableCards.FindAll(cardData => cardData.cardName!="Cloak Block" && cardData.cardName!="Kick"));
            
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