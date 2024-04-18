using System;
using System.Collections.Generic;
using System.Threading;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Random = System.Random;

public class BattleManager: MonoBehaviour
{
    private EventQueue eventQueue = new EventQueue();
    private bool battleEnded = false;
    
    [SerializeField] private GameObject playerCharacterPrefab;
    [SerializeField] private GameObject rewardWindow;
    private DeckManager deckManager;
    private HandManager handManager;
    public List<Enemy> EnemiesInBattle;
    private GameObject playerCharacter;
    private Transform handDisplay;

    public Player PlayerScript;

    public delegate void TurnChangedEventHandler(string turnText, bool isEnemyTurn);

    public event TurnChangedEventHandler OnTurnChange;
    
    void Update()
    {
        if (!battleEnded)
        {
            if (eventQueue.HasEvents())
            {
                Action currentEvent = eventQueue.GetNextEvent();
                currentEvent?.Invoke();
            }
        }
        else
        {
            // Load next scene
        }
    }
    
    public void AddEventToQueue(Action newEvent){
        eventQueue.AddEvent(newEvent);
    }
    
    public void Initialize(List<CardData> deck, List<Enemy> enemies)
    {
        // Find Manager Objects in Scene
        deckManager = new DeckManager(deck);
        handManager = FindObjectOfType<HandManager>();
        handManager.Initialize(deckManager);

        playerCharacter = Instantiate(playerCharacterPrefab, new Vector3(-4.5f, 0, 0), quaternion.identity);
        PlayerScript = playerCharacter.GetComponent<Player>();
        GetComponent<SubscribePlayerUIToStats>().Subscribe();
        PlayerScript.MaxActionPoints = GameStateManager.Instance.MaxActionPoints;
        PlayerScript.ResetActionPoints();
        PlayerScript.CharacterStats.Health = GameStateManager.Instance.CurrentPlayerHealth;
        EnemiesInBattle = new List<Enemy>();

        GenerateEnemies(enemies);
        StartPlayerTurn();
    }

    private void GenerateEnemies(List<Enemy> enemies)
    {
        foreach (Enemy enemy in enemies)
        {
            EnemiesInBattle.Add(Instantiate(enemy, new Vector3(4.5f, 0, 0), quaternion.identity));
        }
    }

    /*
     * Execute a turn for each enemy
     */
    private void EnemyTurn()
    {
        AddEventToQueue(()=>OnTurnChange?.Invoke("Enemy Turn", true));
        foreach (Enemy enemy in EnemiesInBattle)
        {
            // Add event to apply and reduce status effects of enemy
            AddEventToQueue(() =>
            {
                enemy.CharacterStats.Block = 0;
                if (enemy.CharacterStats.Burn > 0)
                {
                    enemy.CharacterStats.Health-=4;
                    enemy.CharacterStats.Burn-=1;
                }
            });

            // Do action
            enemy.PlayEnemyCard();

            // Add event to reduce insight
            AddEventToQueue(() =>
            {
                if (enemy.CharacterStats.Insight > 0)
                {
                    enemy.CharacterStats.Insight-=1;
                }
            });
        }

        StartPlayerTurn();
    }

    /*
     * Start the turn for the player
     */
    private void StartPlayerTurn()
    {
        AddEventToQueue(()=>OnTurnChange?.Invoke("Player Turn",false));
        PlayerScript.ResetActionPoints();
        
        // Add event to apply and reduce status effects of player
        AddEventToQueue(() =>
        {
            PlayerScript.CharacterStats.Block = 0;
            if (PlayerScript.CharacterStats.Burn > 0)
            {
                PlayerScript.CharacterStats.Health -= 4;
                PlayerScript.CharacterStats.Burn -= 1;
            }
        });

        eventQueue.AddEvent(()=> handManager.DrawHand());
    }

    public void EndPlayerTurn()
    {
        // Add event to reduce insight of player
        AddEventToQueue(() =>
        {
            if (PlayerScript.CharacterStats.Insight > 0)
            {
                PlayerScript.CharacterStats.Insight -= 1;
            }
        });

        AddEventToQueue(()=>handManager.DiscardHand());
        EnemyTurn();
    }

    /*
     * Battle has ended, either the player has won or died
     */
    public void EndBattle()
    {
        battleEnded = true;
        handManager.gameObject.SetActive(false);
        //Disable buttons for UI
        
        if (PlayerScript.CharacterStats.Health <= 0)
        {
            // You lost
            return;
        }

        // You won
        GameStateManager.Instance.CurrentPlayerHealth = PlayerScript.CharacterStats.Health;
        rewardWindow.GetComponent<RewardManager>().ShowReward();
    }
}