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
    [SerializeField] private GameObject deathVFX;

    private int currentActionIndex;
    private bool isDead = false;


    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables for the enemy
        CharacterStats.OnHealthChange += CheckForGameOver;
        CharacterStats.Health = maxHealth;

        // Get current action and indicate it
        currentActionIndex = 0;
        actionIndication.GetComponent<MeshRenderer>().material = actionIndicationMaterial.Find(material =>
            material.name == enemyPattern[currentActionIndex].cardType.ToString());
        actionIndication.GetComponentInChildren<TextMeshPro>().text =
            enemyPattern[currentActionIndex].effects[0].payload.ToString();
    }

    /* Play the current selected enemy card*/
    public void PlayEnemyCard()
    {
        EnemyCard enemyCard = enemyPattern[currentActionIndex];

        foreach (CardEffect effect in enemyCard.effects)
        {
            List<Character> targets = new List<Character>();

            switch (effect.effectTarget)
            {
                case CardEffectTarget.Player:
                    targets.Add(BattleManager.Instance.PlayerScript);
                    break;
                case CardEffectTarget.SingleEnemy:
                    targets.Add(this);
                    break;
                case CardEffectTarget.MultipleEnemies:
                    targets.AddRange(BattleManager.Instance.EnemiesInBattle);
                    break;
            }

            // Add vfx to queue -> for status effects
            if (effect.vfxEffect != null && VfxEffects.beforeActionVFX.Contains(effect.effectType))
            {
                BattleManager.Instance.AddEventToQueue(() =>
                    VfxEffects.PlayEffects(effect.vfxEffect, effect.payload, targets.ToArray()));
            }

            // Add event for the effect
            switch (effect.effectType)
            {
                case CardEffectType.Damage:
                    BattleManager.Instance.AddEventToQueue(() =>
                        CardEffectActions.DamageAction(this, effect.payload, effect.ignoreBlock, ref targets));
                    break;

                case CardEffectType.Block:
                    BattleManager.Instance.AddEventToQueue(() =>
                        CardEffectActions.BlockAction(effect.payload, ref targets));
                    break;

                case CardEffectType.Burn:
                    BattleManager.Instance.AddEventToQueue(() =>
                        CardEffectActions.BurnAction(effect.payload, ref targets));
                    break;

                case CardEffectType.Insight:
                    BattleManager.Instance.AddEventToQueue(() =>
                        CardEffectActions.InsightAction(effect.payload, ref targets));
                    break;

                case CardEffectType.AttackDebuff:
                    BattleManager.Instance.AddEventToQueue(() =>
                        CardEffectActions.AttackDebuff(effect.payload, ref targets));
                    break;
            }

            // Add vfx to queue -> for damage effects
            if (effect.vfxEffect != null && !VfxEffects.beforeActionVFX.Contains(effect.effectType))
            {
                BattleManager.Instance.AddEventToQueue(() =>
                    VfxEffects.PlayEffects(effect.vfxEffect, effect.payload, targets.ToArray()));
            }
        }

        // Get next action
        if (++currentActionIndex >= enemyPattern.Count)
        {
            currentActionIndex = 0;
        }

        actionIndication.GetComponent<MeshRenderer>().material = actionIndicationMaterial.Find(material =>
            material.name == enemyPattern[currentActionIndex].cardType.ToString());
        actionIndication.GetComponentInChildren<TextMeshPro>().text =
            enemyPattern[currentActionIndex].effects[0].payload.ToString();
    }

    private void CheckForGameOver(int currentHealth, int maxHealth)
    {
        if (currentHealth <= 0 && !isDead)
        {
            BattleManager.Instance.AddEventToQueue(() =>
            {
                VfxEffects.PlayEffects(deathVFX, 0, this);
                BattleManager.Instance.EnemiesInBattle.Remove(this);
                Destroy(gameObject);
                if (BattleManager.Instance.EnemiesInBattle.Count == 0)
                {
                    BattleManager.Instance.EndBattle();
                }
                //BattleManager.Instance.eventRunning = false;
            });
            isDead = true;
        }
    }
}