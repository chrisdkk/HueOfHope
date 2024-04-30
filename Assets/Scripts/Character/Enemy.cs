using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = System.Random;

public class Enemy : Character
{
    [SerializeField] private int maxHealth = 30;
    [SerializeField] private int enemyTier = 1;
    [SerializeField] private EnemyActionTypes[] possibleEnemyAction;
    [SerializeField] private GameObject actionIndication;
    [SerializeField] private List<Material> actionIndicationMaterial;

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
        CharacterStats.Health = maxHealth;
        enemyPattern = new EnemyPattern(possibleEnemyAction);
        
        // Load and sort all available enemy cards
        EnemyCard[] enemyCards = Resources.LoadAll<EnemyCard>("EnemyCards/");

        // Get card of enemy
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
        
        // Get current action and indicate it
        currentAction = enemyPattern.GetCurrentActionPattern();
        cardPower = random.Next(actionSize);
        EnemyCard card;
        if (currentAction == EnemyActionTypes.Attack)
        {
            card = attackActions.ElementAt(cardPower);
        }
        else
        {
            card = blockActions.ElementAt(cardPower);
        }
        if (currentAction == EnemyActionTypes.Attack)
        {
            actionIndication.GetComponent<MeshRenderer>().material = actionIndicationMaterial.Find(material => material.name == "Attack");
            actionIndication.GetComponentInChildren<TextMeshPro>().text = card.effects[0].payload.ToString();
        }
        else
        {
            actionIndication.GetComponent<MeshRenderer>().material = actionIndicationMaterial.Find(material => material.name == "Block");
            actionIndication.GetComponentInChildren<TextMeshPro>().text = card.effects[0].payload.ToString();
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
        }
        else
        {
            enemyCard = blockActions.ElementAt(cardPower);
        }
        
        
        foreach (CardEffect effect in enemyCard.effects)
        {
            List<Character> targets = new List<Character>();
            // Add event for the effect + Set targets
            switch (effect.effectType)
            {
                case CardEffectType.Damage:
                    targets.Add(BattleManager.Instance.PlayerScript);
                    BattleManager.Instance.AddEventToQueue(()=>CardEffectActions.DamageAction(this, effect.payload, effect.ignoreBlock, ref targets));
                    break;
                
                case CardEffectType.Block:
                    if (effect.multipleTargets)
                    {
                        targets.AddRange(BattleManager.Instance.EnemiesInBattle);
                    }
                    else
                    {
                        targets.Add(this);
                    }
                    BattleManager.Instance.AddEventToQueue(()=>CardEffectActions.BlockAction(effect.payload, ref targets));
                    break;
                
                case CardEffectType.Burn:
                    targets.Add(BattleManager.Instance.PlayerScript);
                    BattleManager.Instance.AddEventToQueue(()=>CardEffectActions.BurnAction(effect.payload, ref targets));
                    break;
                
                case CardEffectType.Insight:
                    if (effect.multipleTargets)
                    {
                        targets.AddRange(BattleManager.Instance.EnemiesInBattle);
                    }
                    else
                    {
                        targets.Add(this);
                    }
                    BattleManager.Instance.AddEventToQueue(()=>CardEffectActions.InsightAction(effect.payload, ref targets));
                    break;
                
                case CardEffectType.AttackDebuff:
                    targets.Add(BattleManager.Instance.PlayerScript);
                    BattleManager.Instance.AddEventToQueue(()=>CardEffectActions.AttackDebuff(effect.payload, ref targets));
                    break;
            }
            // Check if player died
            if (BattleManager.Instance.PlayerScript.CharacterStats.Health<=0)
            {
                BattleManager.Instance.EndBattle();
            }
        }
        // Get next action
        currentAction = enemyPattern.GetCurrentActionPattern();
        cardPower = random.Next(actionSize);
        if (currentAction == EnemyActionTypes.Attack)
        {
            enemyCard = attackActions.ElementAt(cardPower);
        }
        else
        {
            enemyCard = blockActions.ElementAt(cardPower);
        }
        if (currentAction == EnemyActionTypes.Attack)
        {
            actionIndication.GetComponent<MeshRenderer>().material = actionIndicationMaterial.Find(material => material.name == "Attack");
            actionIndication.GetComponentInChildren<TextMeshPro>().text = enemyCard.effects[0].payload.ToString();
        }
        else
        {
            actionIndication.GetComponent<MeshRenderer>().material = actionIndicationMaterial.Find(material => material.name == "Block");
            actionIndication.GetComponentInChildren<TextMeshPro>().text = enemyCard.effects[0].payload.ToString();
        }
    }

    /*Update the healthbar of the enemy*/

}
