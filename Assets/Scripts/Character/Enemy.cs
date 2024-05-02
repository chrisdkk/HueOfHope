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
    [SerializeField] private GameObject actionIndication;
    [SerializeField] private List<Material> actionIndicationMaterial;
    [SerializeField] private List<EnemyCard> enemyPattern = new();
    
    private int currentActionIndex;
    private bool isDead=false;


    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables for the enemy
        CharacterStats.Health = maxHealth;
        CharacterStats.OnHealthChange += CheckForGameOver;

        // Load and sort all available enemy cards
        EnemyCard[] enemyCards = Resources.LoadAll<EnemyCard>("EnemyCards/");

        // Get current action and indicate it
        currentActionIndex = 0;
        actionIndication.GetComponent<MeshRenderer>().material = actionIndicationMaterial.Find(material => material.name == enemyPattern[currentActionIndex].cardType.ToString());
        actionIndication.GetComponentInChildren<TextMeshPro>().text = enemyPattern[currentActionIndex].effects[0].payload.ToString();
    }

    /* Play the current selected enemy card*/
    public void PlayEnemyCard()
    {
        EnemyCard enemyCard = enemyPattern[currentActionIndex];


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
            if (effect.vfxEffect != null)
            {
                BattleManager.Instance.AddEventToQueue(()=>StartCoroutine(VfxEffects.PlayEffects(effect.vfxEffect, targets.ToArray())));   
            }
        }
        // Get next action
        if (++currentActionIndex >= enemyPattern.Count)
        {
            currentActionIndex = 0;
        }
        actionIndication.GetComponent<MeshRenderer>().material = actionIndicationMaterial.Find(material => material.name == enemyPattern[currentActionIndex].cardType.ToString());
        actionIndication.GetComponentInChildren<TextMeshPro>().text = enemyPattern[currentActionIndex].effects[0].payload.ToString();
    }
    
    private void CheckForGameOver(int currentHealth, int maxHealth)
    {
        if (currentHealth <=0 && !isDead)
        {
            BattleManager.Instance.AddEventToQueue(()=>BattleManager.Instance.RemoveEnemy(this));
            isDead = true;
        }
    }
}
