using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;

public class HandManager : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform handTransform;
    [SerializeField] private float horizontalSpacing;
    [SerializeField] private int maxHandSize;
    private DeckManager deck;
    private Stats stats;
    private List<GameObject> cardsInHand = new List<GameObject>();
    
    public void Initialize(DeckManager deckManager)
    {
        deck = deckManager;
    }

    public void DrawHand()
    {
        for (int i = 0; i < maxHandSize; i++)
        {
            AddCardToHand(deck.DrawCard());
        }
    }

    public void DiscardHand()
    {
        for (int i = 0; i < cardsInHand.Count; i++)
        {
            deck.DiscardCard(cardsInHand[i].GetComponent<CardVisual>().cardData);
                Destroy(cardsInHand[i]);
        }
        cardsInHand = new List<GameObject>();
    }

    public void AddCardToHand(CardData cardData)
    {
        GameStateManager.Instance.BattleManager.AddEventToQueue(() =>
        {
            if (cardData == null) return;
            GameObject newCard = Instantiate(cardPrefab, handTransform.position, Quaternion.identity, handTransform);
            cardsInHand.Add(newCard);

            // Set card visuals
            CardVisual cardVisual = newCard.GetComponent<CardVisual>();
            cardVisual.cardData = cardData;
            cardVisual.UpdateCardVisual();
        
            // Set card movement
            CardMovement cardMovement = newCard.GetComponent<CardMovement>();
            cardMovement.OnPlay += PlayCard;
            cardMovement.Initialize(cardData);

            UpdateHandVisuals();
        });
        
    }

    private void PlayCard(GameObject card, GameObject targetedEnemy)
    {
        CardData cardData = card.GetComponent<CardVisual>().cardData;
        bool cardPlayed=true;
        bool alreadyDiscarded = false;

        if (cardData.apCost > GameStateManager.Instance.BattleManager.PlayerScript.CurrentActionPoints)
        {
            return;
        }

        GameStateManager.Instance.BattleManager.PlayerScript.CurrentActionPoints -= cardData.apCost;
        
        foreach (CardEffect effect in cardData.effects)
        {
            List<Character> targets = new List<Character>();
            
            // Add targets
            if (effect.multipleTargets)
            {
                targets.AddRange(GameStateManager.Instance.BattleManager.EnemiesInBattle);
            }else if (targetedEnemy != null)
            {
                targets.Add(targetedEnemy.GetComponent<Enemy>());
            }
            else
            {
                targets.Add(GameStateManager.Instance.BattleManager.PlayerScript);
            }
            
            // Add event for the effect
            switch (effect.effectType)
            {
                case CardEffectType.Damage:
                    // Add event to deal damage
                    GameStateManager.Instance.BattleManager.AddEventToQueue(()=>
                    {
                        CardEffectActions.DamageAction(GameStateManager.Instance.BattleManager.PlayerScript,
                                effect.payload,
                                effect.ignoreBlock || GameStateManager.Instance.BattleManager.PlayerScript.CharacterStats.IgnoreBlockOnNext > 0,
                                ref targets);
                        
                        // Reduce buff ignore block on next attack
                        if (GameStateManager.Instance.BattleManager.PlayerScript.CharacterStats.IgnoreBlockOnNext > 0)
                        {
                            GameStateManager.Instance.BattleManager.PlayerScript.CharacterStats.IgnoreBlockOnNext--;
                        }
                    });
                    break;
             
                case CardEffectType.ShieldBreak:
                    GameStateManager.Instance.BattleManager.AddEventToQueue(()=>CardEffectActions.ShieldBreakAction(GameStateManager.Instance.BattleManager.PlayerScript, effect.payload, ref targets));
                    break;
                
                case CardEffectType.BlockToDamage:
                    // Use block as damage
                    effect.payload = GameStateManager.Instance.BattleManager.PlayerScript.CharacterStats.Block;
                    GameStateManager.Instance.BattleManager.PlayerScript.CharacterStats.Block = 0;
                    
                    // Add event to deal damage
                    GameStateManager.Instance.BattleManager.AddEventToQueue(()=>
                    {
                        CardEffectActions.DamageAction(GameStateManager.Instance.BattleManager.PlayerScript,
                                effect.payload,
                                effect.ignoreBlock || GameStateManager.Instance.BattleManager.PlayerScript.CharacterStats.IgnoreBlockOnNext > 0, 
                                ref targets);
                        
                        // Reduce buff ignore block on next attack
                        if (GameStateManager.Instance.BattleManager.PlayerScript.CharacterStats.IgnoreBlockOnNext > 0)
                        {
                            GameStateManager.Instance.BattleManager.PlayerScript.CharacterStats.IgnoreBlockOnNext--;
                        }
                    });
                    break;
                
                case CardEffectType.MultipliedInsightDamage:
                    // Multiply insight for one attack
                    GameStateManager.Instance.BattleManager.PlayerScript.CharacterStats.Insight*=effect.insightMultiplier;
                    // Add event to deal damage
                    GameStateManager.Instance.BattleManager.AddEventToQueue(()=>
                    {
                        CardEffectActions.DamageAction(GameStateManager.Instance.BattleManager.PlayerScript, 
                            effect.payload, 
                            effect.ignoreBlock || GameStateManager.Instance.BattleManager.PlayerScript.CharacterStats.IgnoreBlockOnNext>0, 
                            ref targets);
                        
                        // Reset insight
                        GameStateManager.Instance.BattleManager.PlayerScript.CharacterStats.Insight/=effect.insightMultiplier;
                        
                        // Reduce buff ignore block on next attack
                        if (GameStateManager.Instance.BattleManager.PlayerScript.CharacterStats.IgnoreBlockOnNext > 0)
                        {
                            GameStateManager.Instance.BattleManager.PlayerScript.CharacterStats.IgnoreBlockOnNext--;
                        }
                    });
                    break;
                
                case CardEffectType.Block:
                    GameStateManager.Instance.BattleManager.AddEventToQueue(()=>CardEffectActions.BlockAction(effect.payload, ref targets));
                    break;
                
                case CardEffectType.Burn:
                    targets.Add(GameStateManager.Instance.BattleManager.PlayerScript);
                    GameStateManager.Instance.BattleManager.AddEventToQueue(()=>CardEffectActions.BurnAction(effect.payload, ref targets));
                    break;
                
                case CardEffectType.Insight:
                    GameStateManager.Instance.BattleManager.AddEventToQueue(()=>CardEffectActions.InsightAction(effect.payload, ref targets));
                    break;
                
                case CardEffectType.AttackDebuff:
                    GameStateManager.Instance.BattleManager.AddEventToQueue(()=>CardEffectActions.AttackDebuff(effect.payload, ref targets));
                    break;
                
                case CardEffectType.Cleanse:
                    GameStateManager.Instance.BattleManager.AddEventToQueue(()=>CardEffectActions.Cleanse(ref targets));
                    break;
                
                case CardEffectType.IgnoreBlockOnNextAttacks:
                    GameStateManager.Instance.BattleManager.AddEventToQueue(()=>GameStateManager.Instance.BattleManager.PlayerScript.CharacterStats.IgnoreBlockOnNext+=effect.payload);
                    break;
                
                case CardEffectType.Draw:
                    for (int i = 0; i < effect.payload; i++)
                    {
                        GameStateManager.Instance.BattleManager.AddEventToQueue(()=>AddCardToHand(deck.DrawCard()));
                    }
                    break;
                
                case CardEffectType.DiscardHand:
                    alreadyDiscarded = true;
                    GameStateManager.Instance.BattleManager.AddEventToQueue(()=>DiscardHand());
                    break;
            }
            
            // Check if an enemy died -> Add event to remove it
            GameStateManager.Instance.BattleManager.AddEventToQueue(()=>
            {
                foreach (Enemy enemy in GameStateManager.Instance.BattleManager.EnemiesInBattle)
                {
                    if (enemy.CharacterStats.Health <= 0)
                    {
                        Destroy(enemy.gameObject);
                    }
                }
                GameStateManager.Instance.BattleManager.EnemiesInBattle.RemoveAll(enemy => enemy.CharacterStats.Health <= 0);
                if (GameStateManager.Instance.BattleManager.EnemiesInBattle.Count==0)
                {
                    GameStateManager.Instance.BattleManager.EndBattle();
                }
                
            });
        }
        // Add event to discard used card
        if (!alreadyDiscarded)
        {
            GameStateManager.Instance.BattleManager.AddEventToQueue(()=> DiscardCard(card));
        }
    }
    
    private void DiscardCard(GameObject card)
    {
        CardData cardData = card.GetComponent<CardVisual>().cardData;
        deck.DiscardCard(cardData);
        cardsInHand.Remove(card);
        Destroy(card);
        
        UpdateHandVisuals();
    }

    private void UpdateHandVisuals()
    {
        int cardCount = cardsInHand.Count;
        float offset = cardPrefab.transform.localScale.x * this.horizontalSpacing;
        for (int i = 0; i < cardCount; i++)
        {
            GameObject card = cardsInHand[i];
            float x = offset * (i - 0.5f * (cardCount - 1));

            card.transform.position = new Vector3(x, handTransform.position.y, i);
        }
    }
}
