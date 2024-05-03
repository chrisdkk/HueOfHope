using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = System.Random;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    private EventQueue eventQueue = new EventQueue();
    public bool eventRunning = false;
    private bool battleEnded = false;

    [SerializeField] private GameObject playerCharacter;
    [SerializeField] private GameObject rewardWindow;
    [SerializeField] private GameObject burnVFX;
    [SerializeField] private List<Transform> EnemyPositions;
    [SerializeField] private GameObject backgroundImage;
    public DeckManager DeckManager { get; private set; }
    public HandManager HandManager { get; private set; }

    public List<Enemy> EnemiesInBattle;

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
        while (!eventRunning && eventQueue.HasEvents() && !battleEnded)
        {
            Action currentEvent = eventQueue.GetNextEvent();
            currentEvent?.Invoke();
        }
    }

    public void AddEventToQueue(Action newEvent)
    {
        eventQueue.AddEvent(newEvent);
    }

    public void Initialize(List<CardData> deck, List<GameObject> enemies, Sprite background)
    {
        OnTurnChange += GameObject.Find("TurnIndication").GetComponent<TurnIndication>().UpdateTurnIndicator;
        // change background when battle starts
        backgroundImage.GetComponentInChildren<Image>().sprite = background;

        // un-end battle when advancing to the next stage
        battleEnded = false;

        // Find Manager Objects in Scene
        DeckManager = new DeckManager(deck);
        HandManager = FindObjectOfType<HandManager>();
        HandManager.Initialize(DeckManager);

        PlayerScript.MaxActionPoints = GameStateManager.Instance.MaxActionPoints;
        PlayerScript.ResetActionPoints();
        PlayerScript.CharacterStats.Health = GameStateManager.Instance.CurrentPlayerHealth;
        EnemiesInBattle = new List<Enemy>();

        AddEventToQueue(() => GenerateEnemies(enemies, EnemyPositions));
        AddEventToQueue(() => StartPlayerTurn());
    }

    private void GenerateEnemies(List<GameObject> enemies, List<Transform> enemyPositions)
    {
        for (int i = 0; i < enemyPositions.Count; i++)
        {
            if ((enemies.Count - 1) >= i)
                EnemiesInBattle.Add(Instantiate(enemies[i], enemyPositions[i].position, Quaternion.identity)
                    .GetComponent<Enemy>());
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
                enemy.CharacterStats.Health -= 4;
                enemy.CharacterStats.Burn -= 1;
                AddEventToQueue(() => StartCoroutine(VfxEffects.PlayEffects(burnVFX, enemy)));
            }

            // Do action
            AddEventToQueue(() => enemy.PlayEnemyCard());

            // Add event to reduce insight
            if (enemy.CharacterStats.Insight > 0)
            {
                enemy.CharacterStats.Insight -= 1;
            }
        }

        AddEventToQueue(() => StartPlayerTurn());
    }

    /*
     * Start the turn for the player
     */
    private void StartPlayerTurn()
    {
        OnTurnChange?.Invoke("Player Turn", false);
        PlayerScript.ResetActionPoints();
        // Add event to apply and reduce status effects of player
        PlayerScript.CharacterStats.Block = 0;
        if (PlayerScript.CharacterStats.Burn > 0)
        {
            PlayerScript.CharacterStats.Health -= 4;
            PlayerScript.CharacterStats.Burn -= 1;
            AddEventToQueue(() => StartCoroutine(VfxEffects.PlayEffects(burnVFX, PlayerScript)));
        }

        AddEventToQueue(() => HandManager.DrawHand());
    }

    public void EndPlayerTurn()
    {
        // Add event to reduce insight of player
        if (PlayerScript.CharacterStats.Insight > 0)
        {
            PlayerScript.CharacterStats.Insight -= 1;
        }

        AddEventToQueue(() => HandManager.DiscardHand());
        AddEventToQueue(() => EnemyTurn());
    }

    /*
     * Battle has ended, either the player has won or died
     */
    public void EndBattle()
    {
        // battleEnded = true;
        AddEventToQueue(() => battleEnded = true);
        // eventQueue.ClearEvents();

        // HandManager.gameObject.SetActive(false);
        HandManager.DiscardHand();
        //Disable buttons for UI

        if (PlayerScript.CharacterStats.Health <= 0)
        {
            return;
        }

        // You won
        GameStateManager.Instance.CurrentPlayerHealth = PlayerScript.CharacterStats.Health;
        rewardWindow.GetComponent<RewardManager>().ShowReward();
    }
}