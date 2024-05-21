using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HandSystem
{
	public class HandManager : MonoBehaviour
	{
		[SerializeField] private CardMovementController stateController;
		[SerializeField] private CardPool cardPool;
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

		private void DrawCards(int numberOfCards, bool startsTurn)
		{
			int cardsDrawnCount = 0;
			for (int i = 0; i < numberOfCards; i++)
			{
				CardData data = deck.DrawCard();
				GameObject newCard = cardPool.GetCard(data);
				cardsInHand.Add(newCard);
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
					cardPool.ReleaseCard(card);
					UpdateCardCost();
				});
			}
		}

		private void UpdateCardCost()
		{
			foreach (GameObject o in cardsInHand)
			{
				CardVisual visual = o.GetComponent<CardVisual>();
				if (visual.isEnabled && visual.CardData.apCost > BattleManager.Instance.PlayerScript.CurrentActionPoints)
				{
					visual.SetDisabled();
				}
				else if (!visual.isEnabled && visual.CardData.apCost <= BattleManager.Instance.PlayerScript.CurrentActionPoints)
				{
					Debug.Log("setenabled");
					visual.SetEnabled();
				}
			}
		}

		private void PlayCard(GameObject card, GameObject targetedEnemy)
		{
			CardData cardData = card.GetComponent<CardVisual>().CardData;
			Player player = BattleManager.Instance.PlayerScript;

            if (cardData.apCost > player.CurrentActionPoints) return;

			player.CurrentActionPoints -= cardData.apCost;

			if (!soundPlayed)
			{
				FindObjectOfType<AudioManager>().Play("CardPlayed");
				soundPlayed = true; // Markiere den Sound als bereits abgespielt
			}

			DiscardCards(new List<GameObject>() {card});
				
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

                // Add vfx to queue
                if (effect.vfxEffect != null && CardEffect.beforeActionVFX.Contains(effect.effectType))
                {
                    BattleManager.Instance.AddEventToQueue(() =>
                        VfxEffects.PlayEffects(effect.vfxEffect, effect.payload, targets.ToArray()));
                }

                // Add event for the effect
                switch (effect.effectType)
                {
                    case CardEffectType.Damage:
                        // Add event to deal damage
                        BattleManager.Instance.AddEventToQueue(() =>
                        {
                            CardEffectActions.DamageAction(player, effect.payload,
                                effect.ignoreBlock || player.CharacterStats.IgnoreBlockOnNext > 0, ref targets);

                            // Reduce buff ignore block on next attack
                            if (player.CharacterStats.IgnoreBlockOnNext > 0)
                            {
                                player.CharacterStats.IgnoreBlockOnNext--;
                            }
                        });
                        break;
                    case CardEffectType.ShieldBreak:
                        BattleManager.Instance.AddEventToQueue(() =>
                            CardEffectActions.ShieldBreakAction(player, effect.payload, ref targets));
                        break;
                    case CardEffectType.BlockToDamage:
                        // Add event to deal damage
                        BattleManager.Instance.AddEventToQueue(() =>
                        {
                            CardEffectActions.DamageAction(player, effect.payload * player.CharacterStats.Block,
                                effect.ignoreBlock || player.CharacterStats.IgnoreBlockOnNext > 0, ref targets);

                            // Reduce buff ignore block on next attack
                            if (player.CharacterStats.IgnoreBlockOnNext > 0)
                            {
                                player.CharacterStats.IgnoreBlockOnNext--;
                            }
                        });
                        break;
                    case CardEffectType.MultipliedInsightDamage:
                        // Multiply insight for one attack
                        player.CharacterStats.Insight *= effect.insightMultiplier;
                        // Add event to deal damage
                        BattleManager.Instance.AddEventToQueue(() =>
                        {
                            CardEffectActions.DamageAction(player, effect.payload,
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
                            CardEffectActions.BlockAction(effect.payload, ref targets));
                        break;
                    case CardEffectType.Burn:
                        BattleManager.Instance.AddEventToQueue(() =>
                            CardEffectActions.BurnAction(effect.payload, ref targets));
                        break;
                    case CardEffectType.InstApplyBurn:
                        BattleManager.Instance.AddEventToQueue(() =>
                            CardEffectActions.InstantApplyBurnAction(ref targets));
                        break;

                    case CardEffectType.TakeOverBurn:
                        otherTargets =
                            new List<Character>(
                                BattleManager.Instance.EnemiesInBattle.Where(enemy => enemy != targets[0]));
                        BattleManager.Instance.AddEventToQueue(() =>
                            CardEffectActions.TakeOverBurn(targets[0], ref otherTargets));
                        break;
                    case CardEffectType.Insight:
                        BattleManager.Instance.AddEventToQueue(() =>
                            CardEffectActions.InsightAction(effect.payload, ref targets));
                        break;
                    case CardEffectType.AttackDebuff:
                        BattleManager.Instance.AddEventToQueue(() =>
                            CardEffectActions.AttackDebuff(effect.payload, ref targets));
                        break;
                    case CardEffectType.Cleanse:
                        BattleManager.Instance.AddEventToQueue(() => CardEffectActions.Cleanse(ref targets));
                        break;
                    case CardEffectType.IgnoreBlockOnNextAttacks:
                        BattleManager.Instance.AddEventToQueue(() =>
                            player.CharacterStats.IgnoreBlockOnNext += effect.payload);
                        break;
                    case CardEffectType.Draw:
                        for (int i = 0; i < effect.payload; i++)
                        {
                            BattleManager.Instance.AddEventToQueue(() => DrawCards(1, false));
                        }

                        break;
                    case CardEffectType.DiscardHand:
                        BattleManager.Instance.AddEventToQueue(() => DiscardCards(cardsInHand));
                        break;
                }

                // Add vfx to queue
                if (effect.vfxEffect != null && !CardEffect.beforeActionVFX.Contains(effect.effectType))
                {
                    BattleManager.Instance.AddEventToQueue(() =>
                        VfxEffects.PlayEffects(effect.vfxEffect, effect.payload, targets.ToArray()));
                }

                if (otherTargets != null)
                {
                    BattleManager.Instance.AddEventToQueue(() =>
                        VfxEffects.PlayEffects(effect.vfxEffect, effect.payload, otherTargets.ToArray()));
                }
            }
        }
    }
}