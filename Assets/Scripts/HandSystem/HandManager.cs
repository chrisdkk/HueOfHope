using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HandSystem
{
    public class HandManager : MonoBehaviour
    {
        [SerializeField] private CardMovementController stateController;
        [SerializeField] private GameObject cardPrefab;
        [SerializeField] private int maxHandSize;

        private DeckManager deck;
        private readonly List<GameObject> cardsInHand = new List<GameObject>();

        public delegate void CardCallback(GameObject card, Action callback);

        public event CardCallback OnDrawCard;
        public event CardCallback OnDiscardCard;
        public event Action OnHandDrawn;

        private bool soundPlayed = false;

        private void Start()
        {
            stateController.OnPlay += PlayCard;
            BattleManager.Instance.OnStartPlayerTurn += () => DrawCards(maxHandSize, true);
            BattleManager.Instance.OnStartEnemyTurn += () => DiscardCards(cardsInHand);
            BattleManager.Instance.OnEndBattle += () => DiscardCards(cardsInHand);
        }

        public void Initialize(DeckManager deckManager)
        {
            deck = deckManager;
        }

        void PlaySound()
        {
            FindObjectOfType<AudioManager>().PlayRandomPowerUp();
        }

        private void DrawCards(int numberOfCards, bool startsTurn)
        {
            BattleManager.Instance.eventRunning = true;
            int cardsDrawnCount = 0;
            for (int i = 0; i < numberOfCards; i++)
            {
                CardData data = deck.DrawCard();
                GameObject newCard = Instantiate(cardPrefab, transform);
                newCard.GetComponent<CardVisual>().LoadCardData(data);
                cardsInHand.Add(newCard);
                UpdateCardCost();
                OnDrawCard?.Invoke(newCard, () =>
                {
                    cardsDrawnCount++;
                    if (cardsDrawnCount >= numberOfCards)
                    {
                        foreach (GameObject card in cardsInHand)
                        {
                            card.layer = LayerMask.NameToLayer("Hand");
                        }

                        if (startsTurn) OnHandDrawn?.Invoke();

                        BattleManager.Instance.eventRunning = false;
                    }
                });
            }
        }

        private void DiscardCards(List<GameObject> cardsToDiscard)
        {
            for (int i = 0; i < Mathf.Min(cardsToDiscard.Count, cardsInHand.Count); i++)
            {
                GameObject card = cardsToDiscard[i];
                if (!cardsInHand.Contains(cardsToDiscard[i])) return;
                card.layer = 0;
                CardData cardData = card.GetComponent<CardVisual>().CardData;
                deck.DiscardCard(cardData);
                OnDiscardCard?.Invoke(card, () =>
                {
                    cardsInHand.Remove(card);
                    BattleManager.Instance.AddEventToQueue(() => Destroy(card));
                });
                UpdateCardCost();
            }
        }

        private void UpdateCardCost()
        {
            foreach (GameObject o in cardsInHand)
            {
                CardVisual visual = o.GetComponent<CardVisual>();
                o.GetComponent<CardCostReduction>().CheckForReducedCosts();
                if (visual.isEnabled &&
                    visual.CardData.apCost - BattleManager.Instance.reduceCardCostsBy >
                    BattleManager.Instance.PlayerScript.CurrentActionPoints)
                {
                    visual.SetDisabled();
                }
                else if (!visual.isEnabled &&
                         visual.CardData.apCost - BattleManager.Instance.reduceCardCostsBy <=
                         BattleManager.Instance.PlayerScript.CurrentActionPoints)
                {
                    visual.SetEnabled();
                }
            }
        }

        private void PlayCard(GameObject card, GameObject targetedEnemy)
        {
            CardData cardData = card.GetComponent<CardVisual>().CardData;
            Player player = BattleManager.Instance.PlayerScript;

            int actualCost = cardData.apCost - BattleManager.Instance.reduceCardCostsBy > 0
                ? cardData.apCost - BattleManager.Instance.reduceCardCostsBy
                : 0;
            if (actualCost > player.CurrentActionPoints) return;

            player.CurrentActionPoints -= actualCost;

            DiscardCards(new List<GameObject>() { card });

            foreach (CardEffect effect in cardData.effects)
            {
                List<Character> targets = new List<Character>();
                List<Character> otherTargets = null;

                // Add targets
                if (effect.effectTarget == CardEffectTarget.MultipleEnemies)
                {
                    targets.AddRange(BattleManager.Instance.EnemiesInBattle);
                }
                else if (effect.effectTarget == CardEffectTarget.SingleEnemy)
                {
                    targets.Add(targetedEnemy.GetComponent<Enemy>());
                }
                else
                {
                    targets.Add(player);
                }

                // Add event for the effect
                switch (effect.effectType)
                {
                    case CardEffectType.Damage:
                        // Add event to deal damage
                        BattleManager.Instance.AddEventToQueue(() =>
                        {
                            CardEffectActions.DamageAction(effect.vfxEffect, player, effect.payload,
                                effect.ignoreBlock || player.CharacterStats.IgnoreBlockOnNext > 0, ref targets);

                            // Reduce buff ignore block on next attack
                            if (player.CharacterStats.IgnoreBlockOnNext > 0)
                            {
                                player.CharacterStats.IgnoreBlockOnNext--;
                            }
                        });
                        FindObjectOfType<AudioManager>().Play("Attack1");
                        break;
                    case CardEffectType.ShieldBreak:
                        BattleManager.Instance.AddEventToQueue(() =>
                            CardEffectActions.ShieldBreakAction(effect.vfxEffect, player, effect.payload, ref targets));
                        FindObjectOfType<AudioManager>().Play("ShieldBreak");
                        break;
                    case CardEffectType.BlockToDamage:
                        // Add event to deal damage
                        BattleManager.Instance.AddEventToQueue(() =>
                        {
                            CardEffectActions.DamageAction(effect.vfxEffect, player,
                                effect.payload * player.CharacterStats.Block,
                                effect.ignoreBlock || player.CharacterStats.IgnoreBlockOnNext > 0, ref targets);

                            // Reduce buff ignore block on next attack
                            if (player.CharacterStats.IgnoreBlockOnNext > 0)
                            {
                                player.CharacterStats.IgnoreBlockOnNext--;
                            }
                        });
                        FindObjectOfType<AudioManager>().Play("Attack1");
                        break;
                    case CardEffectType.MultipliedInsightDamage:
                        // Multiply insight for one attack
                        player.CharacterStats.Insight *= effect.insightMultiplier;
                        // Add event to deal damage
                        BattleManager.Instance.AddEventToQueue(() =>
                        {
                            CardEffectActions.DamageAction(effect.vfxEffect, player, effect.payload,
                                effect.ignoreBlock || player.CharacterStats.IgnoreBlockOnNext > 0, ref targets);

                            // Reset insight
                            player.CharacterStats.Insight /= effect.insightMultiplier;

                            // Reduce buff ignore block on next attack
                            if (player.CharacterStats.IgnoreBlockOnNext > 0)
                            {
                                player.CharacterStats.IgnoreBlockOnNext--;
                            }
                        });
                        break;
                    case CardEffectType.Block:
                        BattleManager.Instance.AddEventToQueue(() =>
                            CardEffectActions.BlockAction(effect.vfxEffect, effect.payload, ref targets));
                        break;
                    case CardEffectType.Burn:
                        BattleManager.Instance.AddEventToQueue(() =>
                            CardEffectActions.BurnAction(effect.vfxEffect, effect.payload, ref targets));
                        break;
                    case CardEffectType.BurnMultipliedByAP:
                        for (int i = player.CurrentActionPoints; i > 0; i--)
                        {
                            BattleManager.Instance.AddEventToQueue(() =>
                                CardEffectActions.BurnMultipliedByAPAction(effect.vfxEffect, ref targets));
                        }

                        player.CurrentActionPoints = 0;
                        UpdateCardCost();
                        break;

                    case CardEffectType.TakeOverBurn:
                        otherTargets =
                            new List<Character>(
                                BattleManager.Instance.EnemiesInBattle.Where(enemy => enemy != targets[0]));
                        BattleManager.Instance.AddEventToQueue(() =>
                            CardEffectActions.TakeOverBurn(effect.vfxEffect, targets[0], ref otherTargets));
                        break;
                    case CardEffectType.Insight:
                        BattleManager.Instance.AddEventToQueue(() =>
                            CardEffectActions.InsightAction(effect.vfxEffect, effect.payload, ref targets));
                        Invoke("PlaySound", 0.8f);
                        break;
                    case CardEffectType.StopInsightDecay:
                        BattleManager.Instance.AddEventToQueue(() => BattleManager.Instance.insightDecay = false);
                        break;
                    case CardEffectType.AttackDebuff:
                        BattleManager.Instance.AddEventToQueue(() =>
                            CardEffectActions.AttackDebuff(effect.vfxEffect, effect.payload, ref targets));
                        break;
                    case CardEffectType.Cleanse:
                        BattleManager.Instance.AddEventToQueue(() =>
                            CardEffectActions.Cleanse(effect.vfxEffect, ref targets));
                        FindObjectOfType<AudioManager>().Play("Chorus");
                        break;
                    case CardEffectType.IgnoreBlockOnNextAttacks:
                        BattleManager.Instance.AddEventToQueue(() =>
                        {
                            VfxEffects.PlayEffects(effect.vfxEffect, effect.payload, targets.ToArray());
                            player.CharacterStats.IgnoreBlockOnNext += effect.payload;
                        });
                        FindObjectOfType<AudioManager>().PlayRandomPowerUp();
                        break;
                    case CardEffectType.Draw:
                        for (int i = 0; i < effect.payload; i++)
                        {
                            BattleManager.Instance.AddEventToQueue(() => DrawCards(1, false));
                        }

                        FindObjectOfType<AudioManager>().Play("CardPlayed");
                        break;
                    case CardEffectType.DiscardHand:
                        BattleManager.Instance.AddEventToQueue(() => DiscardCards(cardsInHand.Where(c => c != card).ToList()));
                        break;
                    case CardEffectType.ReduceCardCostFor1Turn:
                        BattleManager.Instance.AddEventToQueue(() =>
                        {
                            BattleManager.Instance.reduceCardCostsBy = effect.payload;
                            foreach (GameObject o in cardsInHand)
                            {
                                o.GetComponent<CardCostReduction>().CheckForReducedCosts();
                            }

                            UpdateCardCost();
                        });
                        break;
                }
            }
        }
    }
}