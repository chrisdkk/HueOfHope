using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private GameObject playerCharacterPrefab;
    [SerializeField] private GameObject cardPrefab;
    public List<Card> DrawPile { get; private set; }
    public List<GameObject> Hand { get; private set; }
    public List<Card> DiscardPile { get; private set; }
    public List<Enemy> EnemiesInBattle { get; private set; }
    public int CurrentActionPoints { get; set; }
    public Stats PlayerStats;

    public void Initialize(List<Card> deck, List<Enemy> enemies)
    {
        // Player stats, the deck and enemies of this battle
        DrawPile = Shuffle(deck);
        DiscardPile = new List<Card>();
        Hand = new List<GameObject>();
        CurrentActionPoints = GameStateManager.Instance.MaxActionPoints;
        PlayerStats = new Stats();
        PlayerStats.health = GameStateManager.Instance.CurrentPlayerHealth;

        Instantiate(playerCharacterPrefab, new Vector3(-4.5f, 0, 0), quaternion.identity);

        EnemiesInBattle = new List<Enemy>();

        GenerateEnemies(enemies);
        StartPlayerTurn();
    }

    /*
     * Shuffle and return the passed deck
     */
    public List<Card> Shuffle(List<Card> deck)
    {
        Random r = new Random();
        int n = deck.Count;
        while (n > 1)
        {
            n--;
            int k = r.Next(n + 1);
            Card value = deck[k];
            deck[k] = deck[n];
            deck[n] = value;
        }

        return deck;
    }

    /*
     * Draw the upmost card from the draw pile and add it to the hand
     */
    public void DrawCard(float startingPosX, float startingPosZ)
    {
        cardPrefab.GetComponentInChildren<CardVisual>().card = DrawPile[DrawPile.Count - 1];
        Hand.Add(Instantiate(cardPrefab, new Vector3(startingPosX, -2.5f, startingPosZ), quaternion.identity));
        DrawPile.RemoveAt(DrawPile.Count - 1);

        if (DrawPile.Count == 0)
        {
            DrawPile.AddRange(Shuffle(DiscardPile));
        }
    }

    /*
     * Discard a specific card from the hand
     */
    public void DiscardCard(GameObject card)
    {
        DiscardPile.Add(card.GetComponentInChildren<CardVisual>().card);
        Hand.Remove(card);
        Destroy(card);
    }

    /*
     * Discard all cards in the hand
     */
    public void DiscardHand()
    {
        List<GameObject> temp = new List<GameObject>(Hand);

        foreach (GameObject card in temp)
        {
            DiscardCard(card);
        }
    }

    public void GenerateEnemies(List<Enemy> enemies)
    {
        foreach (Enemy enemy in enemies)
        {
            EnemiesInBattle.Add(Instantiate(enemy, new Vector3(4.5f, 0, 0), quaternion.identity));
        }
    }

    /*
     * Execute a turn for each enemy
     */
    public void EnemyTurn()
    {
        foreach (Enemy enemy in EnemiesInBattle)
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
            enemy.EnemyAttack();

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
    public void StartPlayerTurn()
    {
        float cardStartingPosX = -2.5f;
        float cardStartingPosZ = -2f;
        float cardDisplacementZ = 1f;

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

        // Draw until the player has 5 cards in the hand
        while (Hand.Count < 5)
        {
            DrawCard(cardStartingPosX, cardStartingPosZ);
            cardStartingPosX += 1.25f;
            cardStartingPosZ += cardDisplacementZ;

            if (cardStartingPosX == 0f) cardDisplacementZ = -1f;
        }

        CurrentActionPoints = GameStateManager.Instance.MaxActionPoints;
    }

    public void EndPlayerTurn()
    {
        // Reduce insight of player
        if (PlayerStats.insight > 0)
        {
            PlayerStats.insight--;
        }

        DiscardHand();
        EnemyTurn();
    }

    /*
     * Execute the effect of this specific card
     */
    public void ApplyCardEffect(GameObject card, Enemy enemy)
    {
        Stats[] stats = new Stats[] { enemy.stats };

        enemy.stats = card.GetComponentInChildren<CardVisual>().card.ApplyEffects(stats, this)[0];
        enemy.UpdateHealthBar();
        CurrentActionPoints -= card.GetComponentInChildren<CardVisual>().card.apCost;
        DiscardCard(card);
        // Check for aoe effect
        // Execute card effect on either the selected enemy or all enemies
    }


    /*
     * Battle has ended, either the player has won or died
     */
    public void EndBattle()
    {
        if (PlayerStats.health <= 0)
        {
            // You lost
            return;
        }

        // You won
        GameStateManager.Instance.CurrentPlayerHealth = PlayerStats.health;
        ShowReward();
    }

    /*
     * Show three random cards to the player
     */
    public void ShowReward()
    {
        List<Card> rewards = new List<Card>();
        Random r = new Random();
        // Select the three random cards
        // while (rewards.Count < 3)
        // {
        //     int index = r.Next(Deck.AllAvailableCards.Count);
        //     if (!rewards.Contains(Deck.AllAvailableCards[index]))
        //     {
        //         rewards.Add(Deck.AllAvailableCards[index]);
        //     }
        // }
        // Show them in a popup to the player
    }
}