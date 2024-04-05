using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;
    public BattleManager BattleManager { get; private set; }

    [SerializeField] private BattleManager battleManagerPrefab;


    public int CurrentPlayerHealth { get; set; }
    public int MaxPlayerHealth;
    public int MaxActionPoints { get; set; }
    public List<Card> Deck { get; private set; }
    public Card[] AllAvailableCards;
    private List<Enemy> Enemies = new List<Enemy>();

    [SerializeField] public Enemy prototypeEnemy;

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
            MaxPlayerHealth = 30;
            MaxActionPoints = 3;
            CurrentPlayerHealth = MaxPlayerHealth;
            Deck = new List<Card>();

            Enemies.Add(prototypeEnemy);
            //Add standard deck

            AllAvailableCards = Resources.LoadAll<Card>("Cards/");

            for (int i = 0; i < 15; i++)
            {
                Deck.Add(AllAvailableCards[i % 2]);
            }
            
            Debug.Log(Deck.Count);

            BattleManager = Instantiate(battleManagerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            BattleManager.Initialize(Deck, Enemies);
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