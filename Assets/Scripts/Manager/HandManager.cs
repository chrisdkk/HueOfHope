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
    [SerializeField] private Vector3 cardSpawnPos; // Replace with UI -> Screen space to worldspace
    [SerializeField] private Vector3 cardPlayedPos;
    [SerializeField] private Vector3 cardDiscardPos; // Replace with UI -> Screen space to worldspace
    [SerializeField] private Vector3 cardPlayedScale; // Replace with UI -> Screen space to worldspace
    [SerializeField] private Vector3 cardInactiveScale; // Replace with UI -> Screen space to worldspace
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
            AddCardToHand(deck.DrawCard(), true);
        }

        BattleManager.Instance.AddEventToQueue(() => StartCoroutine(UpdateHandVisuals(0.25f)));
    }

    public void DiscardHand()
    {
        for (int i = 0; i < cardsInHand.Count; i++)
        {
            DiscardCard(cardsInHand[i]);
        }

        cardsInHand = new List<GameObject>();
    }

    public void AddCardToHand(CardData cardData, bool drewHand)
    {
        if (cardData == null) return;
        GameObject newCard = Instantiate(cardPrefab, cardSpawnPos, Quaternion.identity, handTransform);
        newCard.transform.localScale = cardInactiveScale;
        cardsInHand.Add(newCard);

        // Set card visuals,
        CardVisual cardVisual = newCard.GetComponent<CardVisual>();
        cardVisual.LoadCardData(cardData);

        // Set card movement
        CardMovement cardMovement = newCard.GetComponent<CardMovement>();
        cardMovement.OnPlay += PlayCard;
        cardMovement.Initialize(cardData);
        if (!drewHand)
        {
            BattleManager.Instance.AddEventToQueue(() => StartCoroutine(UpdateHandVisuals(0.25f)));
        }
    }

    private void PlayCard(GameObject card, GameObject targetedEnemy)
    {
        CardData cardData = card.GetComponent<CardVisual>().CardData;
        bool alreadyDiscarded = false;

        if (cardData.apCost > BattleManager.Instance.PlayerScript.CurrentActionPoints)
        {
            return;
        }

        BattleManager.Instance.PlayerScript.CurrentActionPoints -= cardData.apCost;
        BattleManager.Instance.AddEventToQueue(() =>
            StartCoroutine(card.GetComponent<CardMovement>()
                .HandleMovementFromPositionToPositionState(cardPlayedPos, Quaternion.identity, cardPlayedScale, 0.1f)));

        foreach (CardEffect effect in cardData.effects)
        {
            List<Character> targets = new List<Character>();
            List<Character> otherTargets = null;

            // Add targets
            switch (effect.effectTarget)
            {
                case CardEffectTarget.Player:
                    targets.Add(BattleManager.Instance.PlayerScript);
                    break;
                case CardEffectTarget.SingleEnemy:
                    targets.Add(targetedEnemy.GetComponent<Enemy>());
                    break;
                case CardEffectTarget.MultipleEnemies:
                    targets.AddRange(BattleManager.Instance.EnemiesInBattle);
                    break;
            }

            // Add event for the effect
            switch (effect.effectType)
            {
                case CardEffectType.Damage:
                    // Add event to deal damage
                    BattleManager.Instance.AddEventToQueue(() =>
                    {
                        CardEffectActions.DamageAction(BattleManager.Instance.PlayerScript,
                            effect.payload,
                            effect.ignoreBlock ||
                            BattleManager.Instance.PlayerScript.CharacterStats.IgnoreBlockOnNext > 0,
                            ref targets);

                        // Reduce buff ignore block on next attack
                        if (BattleManager.Instance.PlayerScript.CharacterStats.IgnoreBlockOnNext > 0)
                        {
                            BattleManager.Instance.PlayerScript.CharacterStats.IgnoreBlockOnNext--;
                        }
                    });
                    break;

                case CardEffectType.ShieldBreak:
                    BattleManager.Instance.AddEventToQueue(() =>
                        CardEffectActions.ShieldBreakAction(BattleManager.Instance.PlayerScript, effect.payload,
                            ref targets));
                    break;

                case CardEffectType.BlockToDamage:

                    // Add event to deal damage
                    BattleManager.Instance.AddEventToQueue(() =>
                    {
                        CardEffectActions.DamageAction(BattleManager.Instance.PlayerScript,
                            BattleManager.Instance.PlayerScript.CharacterStats.Block,
                            effect.ignoreBlock ||
                            BattleManager.Instance.PlayerScript.CharacterStats.IgnoreBlockOnNext > 0,
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
                    BattleManager.Instance.PlayerScript.CharacterStats.Insight *= effect.insightMultiplier;
                    // Add event to deal damage
                    BattleManager.Instance.AddEventToQueue(() =>
                    {
                        CardEffectActions.DamageAction(BattleManager.Instance.PlayerScript,
                            effect.payload,
                            effect.ignoreBlock ||
                            BattleManager.Instance.PlayerScript.CharacterStats.IgnoreBlockOnNext > 0,
                            ref targets);

                        // Reset insight
                        BattleManager.Instance.PlayerScript.CharacterStats.Insight /= effect.insightMultiplier;

                        // Reduce buff ignore block on next attack
                        if (BattleManager.Instance.PlayerScript.CharacterStats.IgnoreBlockOnNext > 0)
                        {
                            BattleManager.Instance.PlayerScript.CharacterStats.IgnoreBlockOnNext--;
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
                    otherTargets = new List<Character>(BattleManager.Instance.EnemiesInBattle.Where(enemy=>enemy!=targets[0]));
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
                        BattleManager.Instance.PlayerScript.CharacterStats.IgnoreBlockOnNext += effect.payload);
                    break;

                case CardEffectType.Draw:
                    for (int i = 0; i < effect.payload; i++)
                    {
                        BattleManager.Instance.AddEventToQueue(() => AddCardToHand(deck.DrawCard(), false));
                    }

                    break;

                case CardEffectType.DiscardHand:
                    alreadyDiscarded = true;
                    BattleManager.Instance.AddEventToQueue(() => DiscardHand());
                    break;
            }

            if (effect.vfxEffect != null)
            {
                BattleManager.Instance.AddEventToQueue(() =>
                    StartCoroutine(VfxEffects.PlayEffects(effect.vfxEffect, targets.ToArray())));
            }

            if (otherTargets != null)
            {
                BattleManager.Instance.AddEventToQueue(() =>
                    StartCoroutine(VfxEffects.PlayEffects(effect.vfxEffect, otherTargets.ToArray())));
            }
        }

        // Add event to discard used card
        if (!alreadyDiscarded)
        {
            BattleManager.Instance.AddEventToQueue(() => DiscardCard(card));
        }
    }

    private void DiscardCard(GameObject card)
    {
        CardData cardData = card.GetComponent<CardVisual>().CardData;
        deck.DiscardCard(cardData);
        Quaternion rotation = Quaternion.Euler(0, 0, 180);
        rotation.Normalize();
        BattleManager.Instance.AddEventToQueue(() =>
            StartCoroutine(card.GetComponent<CardMovement>()
                .HandleMovementFromPositionToPositionState(cardDiscardPos, rotation, cardInactiveScale, 0.25f)));
        BattleManager.Instance.AddEventToQueue(() =>
        {
            cardsInHand.Remove(card);
            Destroy(card);

            BattleManager.Instance.AddEventToQueue(() => StartCoroutine(UpdateHandVisuals(0.1f)));
        });
    }

    private IEnumerator UpdateHandVisuals(float duration)
    {
        int cardCount = cardsInHand.Count;
        float offset = cardPrefab.transform.localScale.x * this.horizontalSpacing;

        int midOfCards = (int)Math.Floor(cardCount / 2.0);
        float offsetY = 0.25f;
        float y = handTransform.position.y - 0.5f;

        float rotation = 15f;
        // Get rotation offset depending on how much cards there are
        float rotationOffset = rotation / midOfCards;
        for (int i = 0; i < cardCount; i++)
        {
            GameObject card = cardsInHand[i];
            float x = offset * (i - 0.5f * (cardCount - 1));

            // Offset the card (position and rotation)
            Quaternion targetRot = Quaternion.Euler(0f, 0f, rotation);
            targetRot.Normalize();
            yield return card.GetComponent<CardMovement>()
                .HandleMovementFromPositionToPositionState(new Vector3(x, y, i), targetRot, Vector3.one, duration);
            // Offset changes are depended on even or odd card number
            if (cardCount % 2 == 0)
            {
                // The middle two cards do not need an offset for y
                if (i + 1 != midOfCards)
                {
                    if (i == 0 || i == cardCount - 2)
                    {
                        y += offsetY * 2;
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
                if (i + 1 == midOfCards || i == midOfCards)
                {
                    y += offsetY;
                }
                else
                {
                    y += offsetY * 2;
                }
            }

            // Turn the offset around to create a fan
            if (i + 1 == midOfCards)
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