using System;
using System.Collections.Generic;
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

		private void Start()
		{
			stateController.OnPlay += PlayCard;
			BattleManager.Instance.OnTurnChange += HandleTurnChange;
		}

		public void Initialize(DeckManager deckManager)
		{
			deck = deckManager;
		}

		private void HandleTurnChange(bool isEnemyTurn)
		{
			if (isEnemyTurn) DiscardCards(cardsInHand);
			else DrawCards(maxHandSize);
		}

		private void DrawCards(int numberOfCards)
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
					}
				});
			}
		}

		private void DiscardCards(List<GameObject> cardsToDiscard)
		{
			for (int i = 0; i < cardsToDiscard.Count; i++)
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
				});
			}
		}

		private void PlayCard(GameObject card, GameObject targetedEnemy)
		{
			CardData cardData = card.GetComponent<CardVisual>().CardData;
			Player player = BattleManager.Instance.PlayerScript;

			if (cardData.apCost > player.CurrentActionPoints) return;

			player.CurrentActionPoints -= cardData.apCost;

			BattleManager.Instance.AddEventToQueue(() => DiscardCards(new List<GameObject>() {card}));

			foreach (CardEffect effect in cardData.effects)
			{
				List<Character> targets = new List<Character>();

				// Add targets
				if (effect.multipleTargets)
				{
					targets.AddRange(BattleManager.Instance.EnemiesInBattle);
				}
				else if (targetedEnemy != null)
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
						// Use block as damage
						effect.payload = player.CharacterStats.Block;
						player.CharacterStats.Block = 0;

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
							BattleManager.Instance.AddEventToQueue(() => DrawCards(1));
						}

						break;
					case CardEffectType.DiscardHand:
						BattleManager.Instance.AddEventToQueue(() => DiscardCards(cardsInHand));
						break;
				}

				if (effect.vfxEffect != null)
				{
					BattleManager.Instance.AddEventToQueue(() =>
						StartCoroutine(VfxEffects.PlayEffects(effect.vfxEffect, targets.ToArray())));
				}

				// Check if an enemy died -> Add event to remove it
				BattleManager.Instance.AddEventToQueue(() =>
				{
					foreach (Enemy enemy in BattleManager.Instance.EnemiesInBattle)
					{
						if (enemy.CharacterStats.Health <= 0)
						{
							Destroy(enemy.gameObject);
						}
					}

					BattleManager.Instance.EnemiesInBattle.RemoveAll(enemy => enemy.CharacterStats.Health <= 0);
					if (BattleManager.Instance.EnemiesInBattle.Count == 0)
					{
						BattleManager.Instance.EndBattle();
					}
				});
			}
		}
	}
}