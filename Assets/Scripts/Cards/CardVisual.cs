using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CardVisual : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI cost;
    [SerializeField] private RawImage cardImage;
    [SerializeField] private GameObject disabledOverlay;
    [SerializeField] private Color baseColor;
    [SerializeField] private Color warningColor;

    [SerializeField] private GameObject effectDetailPrefab;
    [SerializeField] private float effectDetailYStart;
    [SerializeField] private float effectDetailYOffset;
    [SerializeField] private float effectDetailX;
    [SerializeField] private float effectDetailScale;
    [SerializeField] private bool addInsight;
    private List<GameObject> effectDetails = new List<GameObject>();

    public CardData CardData { get; private set; }
    public bool isEnabled { get; private set; }

    private void Start()
    {
        isEnabled = true;
        cost.color = baseColor;
        disabledOverlay.SetActive(false);
        BattleManager.Instance.PlayerScript.CharacterStats.OnStatChange += () => LoadCardData(CardData);
    }

    public void LoadCardData(CardData newData)
    {
        if (CardData == null || newData.name != CardData.name)
        {
            CardData = newData;
            // Clear old effect details if existing
            if (effectDetails.Count > 0)
            {
                foreach (GameObject effectDetail in effectDetails)
                {
                    Destroy(effectDetail);
                }

                effectDetails.RemoveRange(0, effectDetails.Count);
            }

            title.SetText(newData.cardName);
            cost.SetText(newData.apCost.ToString());
            cardImage.texture = newData.cardImage;
            GenerateEffectExplanations();
        }
        else
        {
            CardData = newData;
        }

        // Build card text
        string text = "";
        foreach (CardEffect effect in CardData.effects)
        {
            int actualPayload = effect.payload;
            bool damageUpdated = false;

            if (CardEffect.insightAffectedEffects.Contains(effect.effectType) &&
                BattleManager.Instance.PlayerScript.CharacterStats.Insight > 0 && addInsight)
            {
                actualPayload += BattleManager.Instance.PlayerScript.CharacterStats.Insight;
                damageUpdated = true;
            }

            if (effect.effectData != null)
            {
                text += effect.effectData.GetText(actualPayload, damageUpdated, effect.insightMultiplier,
                    effect.effectTarget, true);
                text += "\n";
            }
        }

        description.SetText(text);
    }

    public void SetEnabled()
    {
        isEnabled = true;
        cost.color = baseColor;
        disabledOverlay.SetActive(false);
    }

    public void SetDisabled()
    {
        isEnabled = false;
        cost.color = warningColor;
        disabledOverlay.SetActive(true);
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
                    instObject = Instantiate(effectDetailPrefab, transform, false);
                    break;
                case CardEffectType.Burn:
                    // After merging with new vfx effects -> Burn description should get [NUMBER] and replace it here with BattleManager.Instance.BurnValue
                    instObject = Instantiate(effectDetailPrefab, transform, false);
                    break;
                case CardEffectType.IgnoreBlockOnNextAttacks:
                    instObject = Instantiate(effectDetailPrefab, transform, false);
                    break;
            }

            if (effect.ignoreBlock)
            {
                instObject = Instantiate(effectDetailPrefab, transform, false);
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
                        effect.effectData.effectDescription.Replace("[NUMBER]",
                            GameStateManager.Instance.BurnTickDamage.ToString());
                }

                effectDetails.Add(instObject);
            }
        }
    }

    public void ShowDetails()
    {
        foreach (GameObject effectDetail in effectDetails)
        {
            effectDetail.SetActive(true);
        }
    }

    public void HideDetails()
    {
        foreach (GameObject effectDetail in effectDetails)
        {
            effectDetail.SetActive(false);
        }
    }
}