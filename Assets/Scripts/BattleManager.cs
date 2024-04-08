using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = System.Random;

public class BattleManager: MonoBehaviour
{
    [SerializeField] private GameObject playerCharacterPrefab;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private GameObject rewardPrefab;
    [SerializeField] private int maxHandSize = 5;
    private List<CardData> drawPile;
    private List<Card> hand;
    private List<CardData> discardPile;
    private List<Enemy> enemiesInBattle;
    public int CurrentActionPoints { get; set; }
    public Stats PlayerStats;
    private GameObject playerCharacter;

    private Card selectedCard;
    private Transform handDisplay;

    public void Initialize(List<CardData> deck, List<Enemy> enemies)
    {
        handDisplay = GameObject.Find("HandDisplay").transform;
        // Player stats, the deck and enemies of this battle
        drawPile = Shuffle(deck);
        discardPile = new List<CardData>();
        hand = new List<Card>();
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

    /*
     * Shuffle and return the passed deck
     */
    private List<CardData> Shuffle(List<CardData> deck)
    {
        Random r = new Random();
        int n = deck.Count;
        while (n > 1)
        {
            n--;
            int k = r.Next(n + 1);
            (deck[k], deck[n]) = (deck[n], deck[k]);
        }

        return deck;
    }

    /*
     * Draw the upmost card from the draw pile and add it to the hand
     */
    private void DrawCard()
    {
        if (drawPile.Count > 0)
        {
            CardData drawnCardData = drawPile[0];
            drawPile.RemoveAt(0);

            Card drawnCard = Instantiate(cardPrefab).GetComponent<Card>();
            drawnCard.Initialize(drawnCardData);
            drawnCard.OnClick += SelectCard;
            hand.Add(drawnCard);
            Debug.Log("Drew card: " + drawnCardData.cardName);
        }
        
        if (drawPile.Count == 0)
        {
            drawPile.AddRange(Shuffle(discardPile));
        }
        
        UpdateHandVisuals();
    }

    /*
     * Discard a specific card from the hand
     */
    private void DiscardCard(Card card)
    {
        if (card == selectedCard) selectedCard = null;
        hand.Remove(card);
        discardPile.Add(card.data);
        card.OnClick -= SelectCard;
        Destroy(card.gameObject);
        UpdateHandVisuals();
    }

    /*
     * Discard all cards in the hand
     */
    private void DiscardHand()
    {
        selectedCard = null;
        List<Card> temp = new List<Card>(hand);

        foreach (Card card in temp)
        {
            DiscardCard(card);
        }
        UpdateHandVisuals();
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
    private void StartPlayerTurn()
    {
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
        while (hand.Count < maxHandSize)
        {
            DrawCard();
        }
        UpdateHandVisuals();

        CurrentActionPoints = GameStateManager.Instance.MaxActionPoints;
    }

    public void EndPlayerTurn()
    {
        ClearFocus();
        // Reduce insight of player
        if (PlayerStats.insight > 0)
        {
            PlayerStats.insight--;
        }

        DiscardHand();
        EnemyTurn();
    }

    private void SelectCard(Card card)
    {
        if (selectedCard != null)
        {
            selectedCard.Deselect();
            if (selectedCard == card)
            {
                selectedCard = null;
                ClearFocus();
                return;
            }
        }
        card.Select();
        selectedCard = card;
        SetFocus();
    }

    // This should only be invoked by intended card targets
    private void SelectTarget(GameObject targetObject, bool isPlayer)
    {
        if (selectedCard == null) return;
        if (isPlayer && selectedCard.data.targetSelf)
        {
            PlayerStats = selectedCard.ApplyEffects(new Stats[] {PlayerStats}, this)[0];
        }
        else
        {
            Enemy enemy = targetObject.GetComponent<Enemy>();
            enemy.stats = selectedCard.ApplyEffects(new Stats[] {enemy.stats}, this)[0];
            enemy.UpdateHealthBar();
            if (enemy.stats.health <= 0)
            {
                enemiesInBattle.Remove(enemy);
                Destroy(enemy.gameObject);
                if (enemiesInBattle.Count == 0)
                {
                    EndBattle();
                }
            }
        }
        CurrentActionPoints -= selectedCard.data.apCost;
        DiscardCard(selectedCard);
        ClearFocus();
        if (CurrentActionPoints <= 0)
            EndPlayerTurn();
    }

    private void ClearFocus()
    {
        Target target = playerCharacter.GetComponent<Target>();
        target.SetFocus(false);
        target.OnClick -= SelectTarget;
        foreach (Enemy enemy in enemiesInBattle)
        {
            target = enemy.gameObject.GetComponent<Target>();
            target.SetFocus(false);
            target.OnClick -= SelectTarget;
        }
    }

    private void SetFocus()
    {
        ClearFocus();
        Target target;
        if (selectedCard.data.targetSelf)
        {
            target = playerCharacter.GetComponent<Target>();
            target.SetFocus(true);
            target.OnClick += SelectTarget;
        }
        else
        {
            foreach (Enemy enemy in enemiesInBattle)
            {
                target = enemy.gameObject.GetComponent<Target>();
                target.SetFocus(true);
                target.OnClick += SelectTarget;
            }
        } 
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
        ShowReward();
    }

    /*
     * Show three random cards to the player
     */
    
    public void ShowReward()
    {
        List<CardData> rewards = new List<CardData>();
        Random r = new Random();
        while (rewards.Count < 3)
        {
            int index = r.Next(GameStateManager.Instance.allAvailableCards.Length);
            //if (!rewards.Contains(GameStateManager.AllAvailableCards[index]))
            //{
            rewards.Add(GameStateManager.Instance.allAvailableCards[index]);
            //}
        }

        // Show them in a popup to the player
        int i = 0;
        foreach (Card card in rewardPrefab.GetComponentsInChildren<Card>())
        {
            card.data = rewards[i];
            i++;
        }
        Instantiate(rewardPrefab, new Vector3(0, 0, -4), Quaternion.identity);
    } 

    
    private void UpdateHandVisuals()
    {
        // Calculate spacing between cards
        float cardWidth = cardPrefab.transform.localScale.x;
        float offset = cardWidth * 1.2f;
        float startX = handDisplay.position.x;
     
        // Instantiate visual card prefabs for each card in hand
        for (int i = 0; i < hand.Count; i++)
        {
            float xPos = startX + offset * (i - 0.5f * (hand.Count - 1));

            Card card = hand[i];
            card.transform.position = new Vector3(xPos, handDisplay.position.y, i + 5f);
        }
    }
}