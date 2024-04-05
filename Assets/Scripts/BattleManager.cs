using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class BattleManager:MonoBehaviour
{
    public List<Card> DrawPile{ get; private set; }
    public List<Card> Hand { get; private set; }
    public List<Card> DiscardPile { get; private set; }
    public List<Enemy> EnemiesInBattle { get;private set; }
    public int CurrentPlayerHealth { get; set; }
    public int CurrentActionPoints { get; set; }
    public Stats PlayerStats { get; set; }
    
    public void Initialize(List<Card> deck, List<Enemy> enemies)
    {
        // Player stats, the deck and enemies of this battle
        DrawPile = Shuffle(deck);
        DiscardPile = new List<Card>();
        Hand = new List<Card>();
        CurrentPlayerHealth = GameStateManager.Instance.CurrentPlayerHealth;
        CurrentActionPoints = GameStateManager.Instance.MaxActionPoints;
        PlayerStats = new Stats();
        EnemiesInBattle = enemies;
        StartPlayerTurn();
    }

    /*
     * Shuffle and return the passed deck
     */
    public List<Card> Shuffle(List<Card> deck)
    {
        Random r = new Random();
        int n = deck.Count;
        while (n > 1) {  
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
    public void DrawCard()
    {
        Hand.Add(DrawPile[DrawPile.Count-1]);
        DrawPile.RemoveAt(DrawPile.Count-1);
    }

    /*
     * Discard a specific card from the hand
     */
    public void DiscardCard(Card card)
    {
        DiscardPile.Add(card);
        Hand.Remove(card);
    }

    /*
     * Discard all cards in the hand
     */
    public void DiscardHand()
    {
        foreach (Card card in Hand)
        {
            DiscardCard(card);
        }
    }

    /*
     * Execute a turn for each enemy
     */
    public void EnemyTurn()
    {
        // Reduce insight of player
        foreach (Enemy enemy in EnemiesInBattle)
        {
            // Apply and reduce burn/wound of enemy
        
            // Do action
            
            if (CurrentPlayerHealth <= 0)
            {
                EndBattle();
            }
            
            // Show action for next turn
            // Reduce insight  
        }
        StartPlayerTurn();
    }

    /*
     * Start the turn for the player
     */
    public void StartPlayerTurn()
    {
        // Apply and reduce burn/wound of player
        
        // Draw until the player has 5 cards in the hand
        while (Hand.Count < 5)
        {
            DrawCard();
        }
        CurrentActionPoints = GameStateManager.Instance.MaxActionPoints;
    }

    /*
     * Execute the effect of this specific card
     */
    public void ApplyCardEffect(Card c, Enemy enemy)
    {
        // Check for aoe effect
        // Execute card effect on either the selected enemy or all enemies
    }
    
    
    /*
     * Battle has ended, either the player has won or died
     */
    public void EndBattle()
    {
        if (CurrentPlayerHealth <= 0)
        {
            // You lost
            return;
        }
        
        // You won
        GameStateManager.Instance.CurrentPlayerHealth = CurrentPlayerHealth;
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
        while (rewards.Count < 3)
        {
            int index = r.Next(Deck.AllAvailableCards.Count);
            if (!rewards.Contains(Deck.AllAvailableCards[index]))
            {
                rewards.Add(Deck.AllAvailableCards[index]);
            }
        }
        // Show them in a popup to the player
    }
}
