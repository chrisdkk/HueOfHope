using System;
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

	private EventQueue eventQueue = new EventQueue();
	private bool battleEnded = false;
	public bool eventRunning = false;
	public bool isPaused = false;
	
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

	public void Initialize(List<CardData> deck, List<GameObject> enemies, Sprite background)
	{
		// change background when battle starts
		backgroundImage.GetComponentInChildren<Image>().sprite = background;

		// un-end battle when advancing to the next stage
		battleEnded = false;

		// Find Manager Objects in Scene
		DeckManager = new DeckManager(deck);
		handManager.Initialize(DeckManager);

		PlayerScript.MaxActionPoints = GameStateManager.Instance.MaxActionPoints;
		PlayerScript.ResetActionPoints();
		PlayerScript.CharacterStats.Health = GameStateManager.Instance.CurrentPlayerHealth;
		EnemiesInBattle = new List<Enemy>();

		AddEventToQueue(() => GenerateEnemies(enemies));
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
			// Add event to apply and reduce status effects of enemy
			enemy.CharacterStats.Block = 0;
			if (enemy.CharacterStats.Burn > 0)
			{
				enemy.CharacterStats.Health -= 4;
				enemy.CharacterStats.Burn -= 1;
				StartCoroutine(VfxEffects.PlayEffects(burnVFX, enemy));
			}

			// Do action
			enemy.PlayEnemyCard();

			// Add event to reduce insight
			if (enemy.CharacterStats.Insight > 0)
			{
				enemy.CharacterStats.Insight -= 1;
			}
		}

		AddEventToQueue(StartPlayerTurn);
	}

	/*
	 * Start the turn for the player
	 */
	private void StartPlayerTurn()
	{
		PlayerScript.ResetActionPoints();
		// Add event to apply and reduce status effects of player
		PlayerScript.CharacterStats.Block = 0;
		if (PlayerScript.CharacterStats.Burn > 0)
		{
			PlayerScript.CharacterStats.Health -= 4;
			PlayerScript.CharacterStats.Burn -= 1;
			AddEventToQueue(() => StartCoroutine(VfxEffects.PlayEffects(burnVFX, PlayerScript)));
		}

		AddEventToQueue(() => OnStartPlayerTurn?.Invoke());
	}

	public void EndPlayerTurn()
	{
		// Add event to reduce insight of player
		if (PlayerScript.CharacterStats.Insight > 0)
		{
			PlayerScript.CharacterStats.Insight -= 1;
		}

		EnemyTurn();
		FindObjectOfType<AudioManager>().Play("ButtonClick");
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
			SceneManager.LoadScene("Menu");
			return;
		};
		GameStateManager.Instance.CurrentPlayerHealth = PlayerScript.CharacterStats.Health;
		rewardWindow.GetComponent<RewardManager>().ShowReward();
	}

	public void Pause()
	{
		isPaused = true;
	}

	public void Resume()
	{
		isPaused = false;
	}
}