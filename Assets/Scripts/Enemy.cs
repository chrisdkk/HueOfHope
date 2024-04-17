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
    
    public Stats stats;

    private EnemyPattern enemyPattern;
    private int actionSize = 3;
    private List<EnemyCard> attackActions = new();
    private List<EnemyCard> blockActions = new ();
    private EnemyActionTypes currentAction;
    private int cardPower;
    private Random random = new();


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

        foreach (EnemyCard enemyCard in enemyCards)
        {
            if (enemyCard.tier == enemyTier)
            {
                switch (enemyCard.cardType)
                {
                    case EnemyCardTypes.Attack:
                        attackActions.Add(enemyCard);
                        break;
                    
                    case EnemyCardTypes.Defense:
                        blockActions.Add(enemyCard);
                        break;
                    
                    default:
                        break;
                }
            }
        }
    }

    /* Play the current selected enemy card*/
    public void PlayEnemyCard()
    {
        EnemyCard enemyCard;
        // Play the current action card and target either the player or self
        if (currentAction == EnemyActionTypes.Attack)
        {
            enemyCard = attackActions.ElementAt(cardPower);
            GameStateManager.Instance.BattleManager.PlayCard(stats, false, 0, enemyCard.effects, ref GameStateManager.Instance.BattleManager.PlayerStats);
        }
        else
        {
            enemyCard = blockActions.ElementAt(cardPower);
            GameStateManager.Instance.BattleManager.PlayCard(stats, false, 0, enemyCard.effects, ref stats);
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
