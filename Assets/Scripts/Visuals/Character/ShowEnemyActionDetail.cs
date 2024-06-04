using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShowEnemyActionDetail : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject effectDetail;
    [SerializeField] private TextMeshProUGUI description;

    public void UpdateEnemyActionDetail(List<CardEffect> effects, Character enemy)
    {
        // Build card text
        string text = "";
        foreach (CardEffect effect in effects)
        {
            int actualPayload = effect.payload;
            bool damageUpdated = false;

            if (CardEffect.insightAffectedEffects.Contains(effect.effectType) && enemy.CharacterStats.Insight > 0)
            {
                actualPayload += enemy.CharacterStats.Insight;
                damageUpdated = true;
            }

            if ((effect.effectType == CardEffectType.Damage || effect.effectType == CardEffectType.BlockToDamage ||
                 effect.effectType == CardEffectType.MultipliedInsightDamage) && enemy.CharacterStats.AttackDebuff > 0)
            {
                actualPayload -= enemy.CharacterStats.AttackDebuff;
                actualPayload = actualPayload < 0 ? 0 : actualPayload;
                damageUpdated = true;
            }

            if (effect.effectData != null)
            {
                text += effect.effectData.GetText(effect.payload, actualPayload, damageUpdated,
                    effect.insightMultiplier,
                    effect.effectTarget, false);
                text += "\n";
            }
        }

        description.SetText(text);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        effectDetail.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        effectDetail.SetActive(false);
    }
}