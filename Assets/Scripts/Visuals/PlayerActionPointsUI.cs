using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerActionPointsUI : MonoBehaviour
{
    // Update is called once per frame
    public GameObject[] actionPoints;

    
    private void Awake()
    {
        BattleManager.Instance.PlayerScript.OnActionPointChange += UpdateActionPoints;
    }
    
    public void UpdateActionPoints()
    {
        var textComponent = GetComponent<TextMeshProUGUI>();
        if (textComponent != null)
        {
            textComponent.text = "AP: " + BattleManager.Instance.PlayerScript.CurrentActionPoints +
                                 " / " + GameStateManager.Instance.MaxActionPoints;
        }
        UpdateActionPointUI(); // Refresh the UI
    }

    void UpdateActionPointUI()
    {
        int currentActionPoints = BattleManager.Instance.PlayerScript.CurrentActionPoints;

        for (int i = 0; i < actionPoints.Length; i++)
        {
            SpriteRenderer spriteRenderer = actionPoints[i].GetComponent<SpriteRenderer>(); // Get SpriteRenderer component

            if (spriteRenderer != null)
            {
                Color currentColor = spriteRenderer.color;

                if (i < currentActionPoints)
                {
                    currentColor.a = 1.0f; // Full opacity
                }
                else
                {
                    currentColor.a = 0.2f; // Less opacity
                }

                spriteRenderer.color = currentColor; // Apply updated color

                Debug.Log($"Adjusted opacity of action point {i} to {currentColor.a}");
            }
            else
            {
                Debug.LogWarning($"GameObject at index {i} does not have a SpriteRenderer component.");
            }
        }
    }
}
