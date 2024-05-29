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

            if (CardEffect.insightAffectedEffects.Contains(effect.effectType))
            {
                actualPayload += enemy.CharacterStats.Insight;
            }

            if (effect.effectData != null)
            {
                text += effect.effectData.GetText(effect.payload, actualPayload, false, effect.insightMultiplier,
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