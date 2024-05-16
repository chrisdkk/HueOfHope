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

    [SerializeField] private DeckSystem deckSystem;
    [SerializeField] private MapSystem mapSystem;

    // [SerializeField] public Enemy prototypeEnemy;

    public int CurrentPlayerHealth { get; set; }
    public int maxPlayerHealth;
    public int MaxActionPoints { get; set; }
    public int BurnTickDamage { get; set; }
    public List<CardData> deck;
    public List<CardData> AllAvailableCards;

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

            mapSystem.InitializeMapSystem();

            // set/advance these values in the map view (not here), depending on what "button" you click
            mapSystem.currentChapterIndex = 0;
            mapSystem.currentStageIndex = 0;

            AllAvailableCards = Resources.LoadAll<CardData>("Cards/").ToList();

            // add available cards to deck
            for (int i = 0; i < 5; i++)
            {
                deck.Add(AllAvailableCards.Find(cardData => cardData.cardName == "Cloak Block"));
                deck.Add(AllAvailableCards.Find(cardData => cardData.cardName == "Kick"));
            }
            deck.AddRange(AllAvailableCards.FindAll(cardData => cardData.cardName!="Cloak Block" && cardData.cardName!="Kick"));
            
            // deck system
            deckSystem.InitializeDeck(deck);
            
            battleManager.Initialize(deck, mapSystem.GetEnemies(), mapSystem.GetBackground(), mapSystem.GetStory(), mapSystem.GetHealingOption());
        }
    }

}