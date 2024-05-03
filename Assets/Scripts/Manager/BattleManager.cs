using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;
using Random = System.Random;

public class BattleManager: MonoBehaviour
{
    public static BattleManager Instance;
    
    private EventQueue eventQueue = new EventQueue();
    public bool eventRunning = false;
    private bool battleEnded = false;
    
    [SerializeField] private GameObject playerCharacter;
    [SerializeField] private GameObject rewardWindow;
    [SerializeField] private GameObject burnVFX;
    public DeckManager DeckManager { get; private set; }
    public HandManager HandManager { get; private set; }
    
    public List<Enemy> EnemiesInBattle { get; private set; }

    private Transform handDisplay;

    public Player PlayerScript;

    public delegate void TurnChangedEventHandler(string turnText, bool isEnemyTurn);

    public event TurnChangedEventHandler OnTurnChange;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            PlayerScript = playerCharacter.GetComponent<Player>();
        }    
    }

    void Update()
    {
        // If the event queue is not being processed and there are coroutines in the queue
        if (!eventRunning && eventQueue.HasEvents() && !battleEnded) 
        {
            Action currentEvent = eventQueue.GetNextEvent();
            currentEvent?.Invoke();
        }
    }

    public void AddEventToQueue(Action newEvent){
        eventQueue.AddEvent(newEvent);
    }
    
    public void Initialize(List<CardData> deck, List<Enemy> enemies)
    {
        OnTurnChange += GameObject.Find("TurnIndication").GetComponent<TurnIndication>().UpdateTurnIndicator;
        
        // Find Manager Objects in Scene
        DeckManager = new DeckManager(deck);
        HandManager = FindObjectOfType<HandManager>();
        HandManager.Initialize(DeckManager);

        PlayerScript.MaxActionPoints = GameStateManager.Instance.MaxActionPoints;
        PlayerScript.ResetActionPoints();
        PlayerScript.CharacterStats.Health = GameStateManager.Instance.CurrentPlayerHealth;
        EnemiesInBattle = new List<Enemy>();

        eventQueue.AddEvent(()=>GenerateEnemies(enemies));
        eventQueue.AddEvent(()=>StartPlayerTurn());
    }

    private void GenerateEnemies(List<Enemy> enemies)
    {
        foreach (Enemy enemy in enemies)
        {
            Enemy instEnemy = Instantiate(enemy, new Vector3(4.5f, 0, 0), quaternion.identity);
            instEnemy.transform.localScale *= 0.8f;
            EnemiesInBattle.Add(instEnemy);
            instEnemy = Instantiate(enemy, new Vector3(4.5f, -5, 0), quaternion.identity);
            instEnemy.transform.localScale *= 0.8f;
            EnemiesInBattle.Add(instEnemy);
        }
    }

    /*
     * Execute a turn for each enemy
     */
    private void EnemyTurn()
    {
        OnTurnChange?.Invoke("Enemy Turn", true);
        foreach (Enemy enemy in EnemiesInBattle)
        {
            // Add event to apply and reduce status effects of enemy
            enemy.CharacterStats.Block = 0;
            if (enemy.CharacterStats.Burn > 0) 
            {
                AddEventToQueue(()=>StartCoroutine(VfxEffects.PlayEffects(burnVFX, enemy)));
                enemy.CharacterStats.Health-=GameStateManager.Instance.BurnTickDamage;
                enemy.CharacterStats.Burn-=1;
            }

            if (enemy.CharacterStats.Health > 0)
            {
                // Do action
                AddEventToQueue(()=>enemy.PlayEnemyCard());

                // Add event to reduce insight
                if (enemy.CharacterStats.Insight > 0)
                {
                    enemy.CharacterStats.Insight-=1;
                }
            }
        }

        AddEventToQueue(()=>StartPlayerTurn());
    }

    /*
     * Start the turn for the player
     */
    private void StartPlayerTurn()
    {
        OnTurnChange?.Invoke("Player Turn",false);
        PlayerScript.ResetActionPoints();
        // Add event to apply and reduce status effects of player
        PlayerScript.CharacterStats.Block = 0;
        if (PlayerScript.CharacterStats.Burn > 0)
        {
            AddEventToQueue(()=>StartCoroutine(VfxEffects.PlayEffects(burnVFX, PlayerScript)));
            PlayerScript.CharacterStats.Health -= GameStateManager.Instance.BurnTickDamage;
            PlayerScript.CharacterStats.Burn -= 1;
        }
        eventQueue.AddEvent(()=> HandManager.DrawHand());
    }

    public void EndPlayerTurn()
    {
        // Add event to reduce insight of player
        if (PlayerScript.CharacterStats.Insight > 0)
        {
            PlayerScript.CharacterStats.Insight -= 1;
        }

        AddEventToQueue(()=>HandManager.DiscardHand());
        AddEventToQueue(()=>EnemyTurn());
    }

    /*
     * Battle has ended, either the player has won or died
     */
    public void EndBattle()
    {
        battleEnded = true;
        HandManager.gameObject.SetActive(false);
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

    public void RemoveEnemy(Enemy enemy)
    {
        EnemiesInBattle.Remove(enemy);
        Destroy(enemy.gameObject);
        if (EnemiesInBattle.Count == 0)
        {
            AddEventToQueue(EndBattle);
        }
    }
}