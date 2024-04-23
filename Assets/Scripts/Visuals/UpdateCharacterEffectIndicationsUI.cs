using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpdateCharacterEffectIndicationsUI : MonoBehaviour
{
    [SerializeField] private GameObject burnIndicator;
    [SerializeField] private GameObject insightIndicator;
    [SerializeField] private GameObject ignoreBlockIndicator;
    [SerializeField] private Character character;

    void Start()
    {
        burnIndicator.SetActive(false);
        insightIndicator.SetActive(false);
        ignoreBlockIndicator.SetActive(false);
    }

    /*Update the burn indication of the enemy*/
    public void UpdateBurnIndicator()
    {
        if (character.CharacterStats.Burn > 0)
        {
            burnIndicator.SetActive(true);
            burnIndicator.transform.GetChild(0).GetComponent<TextMeshPro>().text = character.CharacterStats.Burn.ToString();
        }
        else
        {
            burnIndicator.SetActive(false);
        }
    }
    
    /*Update the insight indication of the enemy*/
    public void UpdateInsightIndicator()
    {
        if (character.CharacterStats.Insight > 0)
        {
            insightIndicator.SetActive(true);
            insightIndicator.transform.GetChild(0).GetComponent<TextMeshPro>().text = character.CharacterStats.Insight.ToString();
        }
        else
        {
            insightIndicator.SetActive(false);
        }
    }
    
    /*Update the insight indication of the enemy*/
    public void UpdateIgnoreBlockIndicator()
    {
        if (character.CharacterStats.IgnoreBlockOnNext > 0)
        {
            ignoreBlockIndicator.SetActive(true);
            ignoreBlockIndicator.transform.GetChild(0).GetComponent<TextMeshPro>().text = character.CharacterStats.IgnoreBlockOnNext.ToString();
        }
        else
        {
            ignoreBlockIndicator.SetActive(false);
        }
    }
}
