using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class Enemy : Character
{
    [SerializeField] private int maxHealth = 30;
    [SerializeField] private GameObject actionIndicationImage;
    [SerializeField] private List<Sprite> actionIndicationMaterial;
    [SerializeField] private List<EnemyCard> enemyPattern = new();
    [SerializeField] private GameObject deathVFX;
    [SerializeField] private ShowEnemyActionDetail showEnemyActionDetail;

    private int currentActionIndex;
    public bool isDead = false;


    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables for the enemy
        CharacterStats.OnHealthChange += CheckForGameOver;
        CharacterStats.OnStatChange += UpdateEnemyDamageNumbers;
        CharacterStats.MaxHealth = maxHealth;
        CharacterStats.Health = maxHealth;

        // Get current action and indicate it
        currentActionIndex = 0;
        actionIndicationImage.GetComponent<Image>().sprite = actionIndicationMaterial.Find(sprite =>
            sprite.name == enemyPattern[currentActionIndex].cardType.ToString());
        showEnemyActionDetail.UpdateEnemyActionDetail(enemyPattern[currentActionIndex].effects, this);
    }

    void PlayDebuff()
    {
        FindObjectOfType<AudioManager>().PlayRandomDebuff();
    }

    /* Play the current selected enemy card*/
    public void PlayEnemyCard()
    {
        EnemyCard enemyCard = enemyPattern[currentActionIndex];

        BattleManager.Instance.AddEventToQueue(() =>
        {
            transform.DOMoveX(transform.position.x - .2f, .1125f).OnComplete(
                () => transform.DOMoveX(transform.position.x + .3f, .1125f)
                    .OnComplete(() => transform.DOMoveX(transform.position.x - .1f, .1125f)));
        });

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

            // Add event for the effect
            switch (effect.effectType)
            {
                case CardEffectType.Damage:
                    BattleManager.Instance.AddEventToQueue(() =>
                        CardEffectActions.DamageAction(effect.vfxEffect, this,
                            effect.payload, effect.ignoreBlock,
                            ref targets));
                    FindObjectOfType<AudioManager>().Play("Attack1");
                    break;

                case CardEffectType.Block:
                    BattleManager.Instance.AddEventToQueue(() =>
                        CardEffectActions.BlockAction(effect.vfxEffect, effect.payload, ref targets));
                    break;

                case CardEffectType.Burn:
                    BattleManager.Instance.AddEventToQueue(() =>
                        CardEffectActions.BurnAction(effect.vfxEffect, effect.payload, ref targets));
                    break;

                case CardEffectType.Insight:
                    BattleManager.Instance.AddEventToQueue(() =>
                        CardEffectActions.InsightAction(effect.vfxEffect, effect.payload, ref targets));
                    FindObjectOfType<AudioManager>().PlayRandomPowerUp();
                    break;

                case CardEffectType.AttackDebuff:
                    BattleManager.Instance.AddEventToQueue(() =>
                        CardEffectActions.AttackDebuff(effect.vfxEffect, effect.payload, ref targets));
                    Invoke("PlayDebuff", 0.5f);
                    break;
            }
        }

        // Get next action
        if (++currentActionIndex >= enemyPattern.Count)
        {
            currentActionIndex = 0;
        }

        actionIndicationImage.GetComponent<Image>().sprite = actionIndicationMaterial.Find(sprite =>
            sprite.name == enemyPattern[currentActionIndex].cardType.ToString());
        showEnemyActionDetail.UpdateEnemyActionDetail(enemyPattern[currentActionIndex].effects, this);
    }

    private void CheckForGameOver(int currentHealth, int maxHealth)
    {
        if (currentHealth <= 0 && !isDead)
        {
            GetComponent<Collider>().enabled = false;
            BattleManager.Instance.AddEventToQueue(() =>
            {
                FindObjectOfType<AudioManager>().PlayRandomEnemyDie();
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

    private void UpdateEnemyDamageNumbers()
    {
        showEnemyActionDetail.UpdateEnemyActionDetail(enemyPattern[currentActionIndex].effects, this);
    }
}