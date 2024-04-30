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
            deck.DiscardCard(cardsInHand[i].GetComponent<CardVisual>().CardData);
                Destroy(cardsInHand[i]);
        }
        cardsInHand = new List<GameObject>();
    }

    public void AddCardToHand(CardData cardData)
    {
        BattleManager.Instance.AddEventToQueue(() =>
        {
            if (cardData == null) return;
            GameObject newCard = Instantiate(cardPrefab, handTransform.position, Quaternion.identity, handTransform);
            cardsInHand.Add(newCard);

            // Set card visuals
            CardVisual cardVisual = newCard.GetComponent<CardVisual>();
            cardVisual.LoadCardData(cardData);
        
            // Set card movement
            CardMovement cardMovement = newCard.GetComponent<CardMovement>();
            cardMovement.OnPlay += PlayCard;
            cardMovement.Initialize(cardData);

            UpdateHandVisuals();
        });
        
    }

    private void PlayCard(GameObject card, GameObject targetedEnemy)
    {
        CardData cardData = card.GetComponent<CardVisual>().CardData;
        bool cardPlayed=true;
        bool alreadyDiscarded = false;

        if (cardData.apCost > BattleManager.Instance.PlayerScript.CurrentActionPoints)
        {
            return;
        }

        BattleManager.Instance.PlayerScript.CurrentActionPoints -= cardData.apCost;
        
        foreach (CardEffect effect in cardData.effects)
        {
            List<Character> targets = new List<Character>();
            
            // Add targets
            if (effect.multipleTargets)
            {
                targets.AddRange(BattleManager.Instance.EnemiesInBattle);
            }else if (targetedEnemy != null)
            {
                targets.Add(targetedEnemy.GetComponent<Enemy>());
            }
            else
            {
                targets.Add(BattleManager.Instance.PlayerScript);
            }
            
            // Add event for the effect
            switch (effect.effectType)
            {
                case CardEffectType.Damage:
                    // Add event to deal damage
                    BattleManager.Instance.AddEventToQueue(()=>
                    {
                        CardEffectActions.DamageAction(BattleManager.Instance.PlayerScript,
                                effect.payload,
                                effect.ignoreBlock || BattleManager.Instance.PlayerScript.CharacterStats.IgnoreBlockOnNext > 0,
                                ref targets);

                        // Reduce buff ignore block on next attack
                        if (BattleManager.Instance.PlayerScript.CharacterStats.IgnoreBlockOnNext > 0)
                        {
                            BattleManager.Instance.PlayerScript.CharacterStats.IgnoreBlockOnNext--;
                        }
                    });
                    break;
             
                case CardEffectType.ShieldBreak:
                    BattleManager.Instance.AddEventToQueue(()=>CardEffectActions.ShieldBreakAction(BattleManager.Instance.PlayerScript, effect.payload, ref targets));
                    break;
                
                case CardEffectType.BlockToDamage:
                    // Use block as damage
                    effect.payload = BattleManager.Instance.PlayerScript.CharacterStats.Block;
                    BattleManager.Instance.PlayerScript.CharacterStats.Block = 0;
                    
                    // Add event to deal damage
                    BattleManager.Instance.AddEventToQueue(()=>
                    {
                        CardEffectActions.DamageAction(BattleManager.Instance.PlayerScript,
                                effect.payload,
                                effect.ignoreBlock || BattleManager.Instance.PlayerScript.CharacterStats.IgnoreBlockOnNext > 0, 
                                ref targets);
                        
                        // Reduce buff ignore block on next attack
                        if (BattleManager.Instance.PlayerScript.CharacterStats.IgnoreBlockOnNext > 0)
                        {
                            BattleManager.Instance.PlayerScript.CharacterStats.IgnoreBlockOnNext--;
                        }
                    });
                    break;
                
                case CardEffectType.MultipliedInsightDamage:
                    // Multiply insight for one attack
                    BattleManager.Instance.PlayerScript.CharacterStats.Insight*=effect.insightMultiplier;
                    // Add event to deal damage
                    BattleManager.Instance.AddEventToQueue(()=>
                    {
                        CardEffectActions.DamageAction(BattleManager.Instance.PlayerScript, 
                            effect.payload, 
                            effect.ignoreBlock || BattleManager.Instance.PlayerScript.CharacterStats.IgnoreBlockOnNext>0, 
                            ref targets);
                        
                        // Reset insight
                        BattleManager.Instance.PlayerScript.CharacterStats.Insight/=effect.insightMultiplier;
                        
                        // Reduce buff ignore block on next attack
                        if (BattleManager.Instance.PlayerScript.CharacterStats.IgnoreBlockOnNext > 0)
                        {
                            BattleManager.Instance.PlayerScript.CharacterStats.IgnoreBlockOnNext--;
                        }
                    });
                    break;
                
                case CardEffectType.Block:
                    BattleManager.Instance.AddEventToQueue(()=>CardEffectActions.BlockAction(effect.payload, ref targets));
                    break;
                
                case CardEffectType.Burn:
                    BattleManager.Instance.AddEventToQueue(()=>CardEffectActions.BurnAction(effect.payload, ref targets));
                    break;
                
                case CardEffectType.Insight:
                    BattleManager.Instance.AddEventToQueue(()=>CardEffectActions.InsightAction(effect.payload, ref targets));
                    break;
                
                case CardEffectType.AttackDebuff:
                    BattleManager.Instance.AddEventToQueue(()=>CardEffectActions.AttackDebuff(effect.payload, ref targets));
                    break;
                
                case CardEffectType.Cleanse:
                    BattleManager.Instance.AddEventToQueue(()=>CardEffectActions.Cleanse(ref targets));
                    break;
                
                case CardEffectType.IgnoreBlockOnNextAttacks:
                    BattleManager.Instance.AddEventToQueue(()=>BattleManager.Instance.PlayerScript.CharacterStats.IgnoreBlockOnNext+=effect.payload);
                    break;
                
                case CardEffectType.Draw:
                    for (int i = 0; i < effect.payload; i++)
                    {
                        BattleManager.Instance.AddEventToQueue(()=>AddCardToHand(deck.DrawCard()));
                    }
                    break;
                
                case CardEffectType.DiscardHand:
                    alreadyDiscarded = true;
                    BattleManager.Instance.AddEventToQueue(()=>DiscardHand());
                    break;
            }

            if (effect.vfxEffect != null)
            {
                BattleManager.Instance.AddEventToQueue(()=>StartCoroutine(VfxEffects.PlayEffects(effect.vfxEffect, targets.ToArray())));   
            }

            // Check if an enemy died -> Add event to remove it
            BattleManager.Instance.AddEventToQueue(()=>
            {
                foreach (Enemy enemy in BattleManager.Instance.EnemiesInBattle)
                {
                    if (enemy.CharacterStats.Health <= 0)
                    {
                        Destroy(enemy.gameObject);
                    }
                }
                BattleManager.Instance.EnemiesInBattle.RemoveAll(enemy => enemy.CharacterStats.Health <= 0);
                if (BattleManager.Instance.EnemiesInBattle.Count==0)
                {
                    BattleManager.Instance.EndBattle();
                }
                
            });
        }
        // Add event to discard used card
        if (!alreadyDiscarded)
        {
            BattleManager.Instance.AddEventToQueue(()=> DiscardCard(card));
        }
    }

    private void DiscardCard(GameObject card)
    {
        CardData cardData = card.GetComponent<CardVisual>().CardData;
        deck.DiscardCard(cardData);
        cardsInHand.Remove(card);
        Destroy(card);
        
        UpdateHandVisuals();
    }

    private void UpdateHandVisuals()
    {
        int cardCount = cardsInHand.Count;
        float offset = cardPrefab.transform.localScale.x * this.horizontalSpacing;

        int midOfCards = (int)Math.Floor(cardCount / 2.0);
        float offsetY = 0.25f;
        float y = handTransform.position.y-0.5f;

        float rotation = 15f;
        // Get rotation offset depending on how much cards there are
        float rotationOffset = rotation/midOfCards;
        for (int i = 0; i < cardCount; i++)
        {
            GameObject card = cardsInHand[i];
            float x = offset * (i - 0.5f * (cardCount - 1));
            
            // Offset the card (position and rotation)
            card.transform.position = new Vector3(x, y, i);
            card.transform.eulerAngles = new Vector3(0,0,rotation);
            
            // Offset changes are depended on even or odd card number
            if (cardCount % 2 == 0)
            {
                // The middle two cards do not need an offset for y
                if (i+1 != midOfCards)
                {
                    if (i == 0 || i==cardCount-2)
                    {
                        y += offsetY*2;
                    }
                    else
                    {
                        y += offsetY;
                    }
                }
            }
            else
            {
                // Before and after the middle card to offset has to be lower
                if (i+1 == midOfCards || i == midOfCards)
                {
                    y += offsetY;
                }
                else
                {
                    y += offsetY*2;   
                }
            }
            
            // Turn the offset around to create a fan
            if (i+1== midOfCards)
            {
                offsetY *= -1;
            }
            
            rotation -= rotationOffset;

            // Even cards are all tilted, there is no card with 0 rotation
            if (cardCount % 2 == 0 && rotation == 0)
            {
                rotation -= rotationOffset;
            }
        }
    }
}
