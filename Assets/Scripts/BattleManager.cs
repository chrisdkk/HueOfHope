using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = System.Random;

public class BattleManager: MonoBehaviour
{
    [SerializeField] private GameObject playerCharacterPrefab;
    [SerializeField] private GameObject rewardWindow;
    private DeckManager deckManager;
    private HandManager handManager;
    private List<Enemy> enemiesInBattle;
    public int CurrentActionPoints { get; set; }
    public Stats PlayerStats;
    private GameObject playerCharacter;

    private Card selectedCard;
    private Transform handDisplay;

    public void Initialize(List<CardData> deck, List<Enemy> enemies)
    {
        // Find Manager Objects in Scene
        deckManager = new DeckManager(deck);
        handManager = FindObjectOfType<HandManager>();
        handManager.Initialize(deckManager);
        CurrentActionPoints = GameStateManager.Instance.MaxActionPoints;
        PlayerStats = new Stats
        {
            health = GameStateManager.Instance.CurrentPlayerHealth
        };

        playerCharacter = Instantiate(playerCharacterPrefab, new Vector3(-4.5f, 0, 0), quaternion.identity);

        enemiesInBattle = new List<Enemy>();

        GenerateEnemies(enemies);
        StartPlayerTurn();
    }

    private void GenerateEnemies(List<Enemy> enemies)
    {
        foreach (Enemy enemy in enemies)
        {
            enemiesInBattle.Add(Instantiate(enemy, new Vector3(4.5f, 0, 0), quaternion.identity));
        }
    }

    /*
     * Execute a turn for each enemy
     */
    private void EnemyTurn()
    {
        foreach (Enemy enemy in enemiesInBattle)
        {
            // Apply and reduce status effects of enemy
            enemy.stats.defense = 0;
            if (enemy.stats.burn > 0)
            {
                enemy.stats.health -= 4;
                enemy.stats.burn--;
            }

            if (enemy.stats.wound > 0)
            {
                enemy.stats.health -= 5;
                enemy.stats.wound--;
            }

            // Do action
            enemy.PlayEnemyCard();

            if (PlayerStats.health <= 0)
            {
                EndBattle();
            }

            // Show action for next turn
            // Reduce insight
            if (enemy.stats.insight > 0)
            {
                enemy.stats.insight--;
            }
        }

        StartPlayerTurn();
    }

    /*
     * Start the turn for the player
     */
    private void StartPlayerTurn()
    {
        GameObject.Find("HealthBar").GetComponent<HealthMonitor>().UpdatePlayerAnimation(PlayerStats.health);
        // Apply and reduce status effects of player
        PlayerStats.defense = 0;
        if (PlayerStats.burn > 0)
        {
            PlayerStats.health -= 4;
            PlayerStats.burn--;
        }

        if (PlayerStats.wound > 0)
        {
            PlayerStats.health -= 5;
            PlayerStats.wound--;
        }

        handManager.DrawHand();

        CurrentActionPoints = GameStateManager.Instance.MaxActionPoints;
    }

    public void EndPlayerTurn()
    {
        // Reduce insight of player
        if (PlayerStats.insight > 0)
        {
            PlayerStats.insight--;
        }

        handManager.DiscardHand();
        EnemyTurn();
    }

    /*
     * Battle has ended, either the player has won or died
     */
    private void EndBattle()
    {
        if (PlayerStats.health <= 0)
        {
            // You lost
            return;
        }

        // You won
        GameStateManager.Instance.CurrentPlayerHealth = PlayerStats.health;
        rewardWindow.GetComponent<RewardManager>().ShowReward();
    }

}