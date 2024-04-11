using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private int maxHealth = 30;
    [SerializeField]
    private int enemyTier = 1;
    [SerializeField]
    private GameObject healthBarUI;
    [SerializeField]
    private EnemyActionTypes[] possibleEnemyAction;
    
    public Stats stats = new Stats();

    private EnemyPattern enemyPattern;
    private int actionSize = 3;
    private List<EnemyCard> attackActions = new List<EnemyCard>();
    private List<EnemyCard> blockActions = new List<EnemyCard>();
    private EnemyActionTypes currentAction;
    private int cardPower;
    private Random random = new Random();


    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables for the enemy
        stats.health = maxHealth;
        enemyPattern = new EnemyPattern(possibleEnemyAction);
        currentAction = enemyPattern.GetCurrentActionPattern();
        cardPower = random.Next(actionSize);
        
        // Load and sort all available enemy cards
        EnemyCard[] enemyCards = Resources.LoadAll<EnemyCard>("EnemyCards/");
        Dictionary<int, List<EnemyCard>> allEnemyAttackCards = new Dictionary<int, List<EnemyCard>>();
        Dictionary<int, List<EnemyCard>> allEnemyBlockCards = new Dictionary<int, List<EnemyCard>>();

        for (int i = 1; i <= 3; i++)
        {
            allEnemyAttackCards.Add(i, new List<EnemyCard>());
            allEnemyBlockCards.Add(i, new List<EnemyCard>());
        }
        
        foreach (EnemyCard enemyCard in enemyCards)
        {
            if (enemyCard.tier == enemyTier)
            {
                switch (enemyCard.cardType)
                {
                    case EnemyCardTypes.Attack:
                        allEnemyAttackCards[enemyCard.power].Add(enemyCard);
                        break;
                    
                    case EnemyCardTypes.Defense:
                        allEnemyBlockCards[enemyCard.power].Add(enemyCard);
                        break;
                    
                    default:
                        break;
                }
            }
        }

        // Select 3 random cards for each enemy action type
        for (int i = 1; i <= 3; i++)
        {
            var randomIndex = random.Next(allEnemyAttackCards[i].Count);
            attackActions.Add(allEnemyAttackCards[i].ElementAt(randomIndex));
            
            randomIndex = random.Next(allEnemyBlockCards[i].Count);
            blockActions.Add(allEnemyBlockCards[i].ElementAt(randomIndex));
        }

        foreach (EnemyCard card in attackActions)
        {
            Debug.Log("Attack:" + card);
        }
        
        foreach (EnemyCard card in blockActions)
        {
            Debug.Log("Block:" + card);
        }
    }

    /* Play the current selected enemy card*/
    public void PlayEnemyCard()
    {
        // Load effects and targets for the card
        CardEffect[] effects;
        Stats[] targets = new Stats[1]; // Change if enemies can shield all enemies
        switch (currentAction)
        {
            case EnemyActionTypes.Attack:
                effects = attackActions.ElementAt(cardPower).effects;
                targets[1] = GameStateManager.Instance.BattleManager.PlayerStats;
                break;
            
            case EnemyActionTypes.Block:
                effects = blockActions.ElementAt(cardPower).effects;
                targets[1] = stats;
                break;
            default:
                effects = null;
                break;
        }

        // Apply all effects
        foreach (CardEffect effect in effects)
        {
            effect.Apply(targets, GameStateManager.Instance.BattleManager);
        }
        
        currentAction = enemyPattern.GetCurrentActionPattern();
        cardPower = random.Next(actionSize);
    }

    /*Update the healthbar of the enemy*/
    public void UpdateHealthBar()
    {
        healthBarUI.transform.localScale = new Vector3((float)stats.health / (float)maxHealth, 0.04f, 0.5f);
    }
}
