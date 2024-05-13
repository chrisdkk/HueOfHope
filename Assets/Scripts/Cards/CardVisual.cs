using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CardVisual : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI cost;
    [SerializeField] private RawImage cardImage;
    [SerializeField] private GameObject effectDetail;
    [SerializeField] private float effectDetailYStart;
    [SerializeField] private float effectDetailYOffset;
    [SerializeField] private float effectDetailX;
    [SerializeField] private float effectDetailScale;
    public CardData CardData { get; private set; }

    private List<GameObject> effectDetails = new List<GameObject>();

    public void LoadCardData(CardData newData)
    {
        CardData = newData;
        title.SetText(CardData.cardName);
        
        // Build card text
        string text = "";
        foreach (CardEffect effect in CardData.effects)
        {
            int actualPayload = effect.payload;
            bool damageUpdated = false;

            if (CardEffect.insightAffectedEffects.Contains(effect.effectType) &&
                BattleManager.Instance.PlayerScript.CharacterStats.Insight > 0)
            {
                actualPayload += BattleManager.Instance.PlayerScript.CharacterStats.Insight;
                damageUpdated = true;
            }

            if (effect.effectData != null)
            {
                text += effect.effectData.GetText(actualPayload, damageUpdated, effect.insightMultiplier, effect.effectTarget);
                text += "\n";
            }
        }

        description.SetText(text);
        cost.SetText(CardData.apCost.ToString());
        cardImage.texture = CardData.cardImage;
        GenerateEffectExplanations();
    }

    public void GenerateEffectExplanations()
    {
        float currY = effectDetailYStart;
        // Loop through list containing each effect type only once
        foreach (var effect in CardData.effects.GroupBy(effect => effect.effectType).Select(e => e.First()))
        {
            GameObject instObject = null;
            switch (effect.effectType)
            {
                case CardEffectType.Insight:
                    instObject = Instantiate(effectDetail, transform, false);
                    break;
                case CardEffectType.Burn:
                    // After merging with new vfx effects -> Burn description should get [NUMBER] and replace it here with BattleManager.Instance.BurnValue
                    instObject = Instantiate(effectDetail, transform, false);
                    break;
                case CardEffectType.IgnoreBlockOnNextAttacks:
                    instObject = Instantiate(effectDetail, transform, false);
                    break;
            }

            if (effect.ignoreBlock)
            {
                instObject = Instantiate(effectDetail, transform, false);
            }

            if (instObject != null)
            {
                instObject.transform.localScale *= effectDetailScale;
                instObject.transform.localPosition = new Vector3(effectDetailX, currY, 0);
                currY += effectDetailYOffset;
                instObject.SetActive(false);

                if (effect.effectData != null)
                {
                    instObject.transform.Find("Title").GetComponent<TextMeshProUGUI>().text = effect.effectData.title;
                    instObject.transform.Find("Description").GetComponent<TextMeshProUGUI>().text =
                        effect.effectData.effectDescription;
                }

                effectDetails.Add(instObject);
            }
        }
    }

    public void ToggleDetails()
    {
        foreach (GameObject effectDetail in effectDetails)
        {
            effectDetail.SetActive(!effectDetail.activeSelf);
        }
    }
}