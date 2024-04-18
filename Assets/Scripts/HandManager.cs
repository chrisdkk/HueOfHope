using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                    GameStateManager.Instance.BattleManager.AddEventToQueue(()=>CardEffectActions.DamageAction(GameStateManager.Instance.BattleManager.PlayerScript, effect.payload, effect.ignoreBlock, ref targets));
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
            }

            // Check if an enemy died -> Add event to remove it
            foreach (Enemy enemy in GameStateManager.Instance.BattleManager.EnemiesInBattle)
            {
                if (enemy.CharacterStats.Health <= 0)
                {
                    GameStateManager.Instance.BattleManager.AddEventToQueue(()=>
                    {
                        Destroy(enemy.gameObject);
                        GameStateManager.Instance.BattleManager.EnemiesInBattle.RemoveAll(enemy => enemy.CharacterStats.Health <= 0);
                        if (GameStateManager.Instance.BattleManager.EnemiesInBattle.Count==0)
                        {
                            GameStateManager.Instance.BattleManager.EndBattle();
                        }
                    });
                }
            }
        }
        // Add event to discard used card
        GameStateManager.Instance.BattleManager.AddEventToQueue(()=> DiscardCard(card));
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
