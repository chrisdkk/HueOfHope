using System;
using System.Collections;
using System.Collections.Generic;
using HandSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    [SerializeField] private GameObject playerCharacter;
    [SerializeField] private GameObject rewardWindow;
    [SerializeField] private GameObject burnVFX;
    [SerializeField] private List<Transform> enemyPositions;
    [SerializeField] private GameObject backgroundImage;
    [SerializeField] private HandManager handManager;
    [SerializeField] public GameObject dmbNumberEffect;
    [SerializeField] public GameObject blockNumberEffect;

    private EventQueue eventQueue = new EventQueue();
    private bool battleEnded = false;
    public bool eventRunning = false;
    public bool isPaused = false;
    public bool insightDecay = true;
    public int reduceCardCostsBy = 0;

    public DeckManager DeckManager { get; private set; }
    public List<Enemy> EnemiesInBattle { get; private set; }
    public Player PlayerScript { get; private set; }

    public event Action OnStartPlayerTurn;
    public event Action OnStartEnemyTurn;
    public event Action OnEndBattle;

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

    public void Initialize(List<CardData> deck, List<GameObject> enemies, Sprite background, string storyText)
    {
        // change background when battle starts
        backgroundImage.GetComponentInChildren<Image>().sprite = background;

        // un-end battle when advancing to the next stage
        battleEnded = false;

        rewardWindow.GetComponent<RewardManager>().Initialize(storyText);

        PlayerScript.ResetBuffsAndDebuffs();
        PlayerScript.MaxActionPoints = GameInitializer.Instance.MaxActionPoints;
        PlayerScript.ResetActionPoints();
        PlayerScript.CharacterStats.MaxHealth = GameInitializer.Instance.maxPlayerHealth;
        PlayerScript.CharacterStats.Health = GameInitializer.Instance.CurrentPlayerHealth;
        insightDecay = true;
        EnemiesInBattle = new List<Enemy>();

        AddEventToQueue(() => GenerateEnemies(enemies));

        // If player has already a deck -> Start battle -> Else wait for deck selection
        if (deck.Count > 0)
        {
            // Find Manager Objects in Scene
            DeckManager = new DeckManager(deck);
            handManager.Initialize(DeckManager);
            AddEventToQueue(StartPlayerTurn);
        }
    }

    public void InitializeStarterDeck(List<CardData> deck)
    {
        // Find Manager Objects in Scene
        DeckManager = new DeckManager(deck);
        handManager.Initialize(DeckManager);
        AddEventToQueue(StartPlayerTurn);
    }

    private void GenerateEnemies(List<GameObject> enemies)
    {
        for (int i = 0; i < enemyPositions.Count; i++)
        {
            if ((enemies.Count - 1) >= i)
            {
                GameObject instEnemy = Instantiate(enemies[i], enemyPositions[i].position, Quaternion.identity);
                instEnemy.transform.localScale *= 0.8f;
                EnemiesInBattle.Add(instEnemy.GetComponent<Enemy>());
            }
        }
    }

    /*
     * Execute a turn for each enemy
     */
    private void EnemyTurn()
    {
        OnStartEnemyTurn?.Invoke();
        foreach (Enemy enemy in EnemiesInBattle)
        {
            // Reduce status effects of enemy
            enemy.CharacterStats.Block = 0;
            if (enemy.CharacterStats.Burn > 0 && !enemy.isDead)
            {
                AddEventToQueue(() =>
                {
                    enemy.CharacterStats.Health -= enemy.CharacterStats.Burn;
                    VfxEffects.PlayEffects(burnVFX, enemy.CharacterStats.Burn, enemy);
                    VfxEffects.PlayEffects(dmbNumberEffect, enemy.CharacterStats.Burn,
                        enemy);
                    enemy.CharacterStats.Burn -= 1;
                });
            }

            if (enemy.isDead) continue;

            // Do action
            enemy.PlayEnemyCard();

            AddEventToQueue(() =>
            {
                // Reduce enemy status effects
                enemy.CharacterStats.AttackDebuff = 0;
                if (enemy.CharacterStats.Insight > 0)
                {
                    enemy.CharacterStats.Insight -= 1;
                }
            });
        }

        AddEventToQueue(StartPlayerTurn);
    }

    /*
     * Start the turn for the player
     */
    private void StartPlayerTurn()
    {
        PlayerScript.ResetActionPoints();
        // Reduce status effects of player
        PlayerScript.CharacterStats.Block = 0;
        if (PlayerScript.CharacterStats.Burn > 0)
        {
            AddEventToQueue(() =>
            {
                PlayerScript.CharacterStats.Health -= PlayerScript.CharacterStats.Burn;
                VfxEffects.PlayEffects(burnVFX, PlayerScript.CharacterStats.Burn, PlayerScript);
                VfxEffects.PlayEffects(Instance.dmbNumberEffect, PlayerScript.CharacterStats.Burn,
                    PlayerScript);
                PlayerScript.CharacterStats.Burn -= 1;
            });
        }

        AddEventToQueue(() => OnStartPlayerTurn?.Invoke());
    }

    public void EndPlayerTurn()
    {
        // Reduce status effects of player
        PlayerScript.CharacterStats.AttackDebuff = 0;
        reduceCardCostsBy = 0;
        if (PlayerScript.CharacterStats.Insight > 0 && insightDecay)
        {
            PlayerScript.CharacterStats.Insight -= 1;
        }

        EnemyTurn();
    }

    /*
     * Battle has ended, either the player has won or died
     */
    public void EndBattle()
    {
        OnEndBattle?.Invoke();
        battleEnded = true;
        eventQueue.ClearEvents();
        if (PlayerScript.CharacterStats.Health <= 0)
        {
            AudioManager audioManager = FindObjectOfType<AudioManager>();
            if (audioManager != null)
            {
                audioManager.StopAllSounds();
            }

            SceneManager.LoadScene("Lose");
            return;
        }

        GameInitializer.Instance.CurrentPlayerHealth = PlayerScript.CharacterStats.Health;
        rewardWindow.GetComponent<RewardManager>().StartRewardManager();
        FindObjectOfType<AudioManager>().Play("CompleteStage");
    }

    public void Pause()
    {
        isPaused = true;
        FindObjectOfType<AudioManager>().Play("ButtonClick2");
    }

    public void Resume()
    {
        isPaused = false;
        FindObjectOfType<AudioManager>().Play("ButtonClick2");
    }
}