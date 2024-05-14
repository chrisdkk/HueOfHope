using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerActionPointsUI : MonoBehaviour
{
	// Update is called once per frame
	public GameObject[] actionPoints;

	private void Start()
	{
		BattleManager.Instance.PlayerScript.OnActionPointChange += UpdateActionPoints;
	}

	private void UpdateActionPoints()
	{
		var textComponent = GetComponent<TextMeshProUGUI>();
		if (textComponent != null)
		{
			textComponent.text = "AP: " + BattleManager.Instance.PlayerScript.CurrentActionPoints + " / " +
			                     GameStateManager.Instance.MaxActionPoints;
		}

		UpdateActionPointUI(); // Refresh the UI
	}

	private void UpdateActionPointUI()
	{
		int currentActionPoints = BattleManager.Instance.PlayerScript.CurrentActionPoints;

		for (int i = 0; i < actionPoints.Length; i++)
		{
			SpriteRenderer
				spriteRenderer = actionPoints[i].GetComponent<SpriteRenderer>(); // Get SpriteRenderer component
			if (spriteRenderer != null)
			{
				spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b,
					i < currentActionPoints ? 1.0f : 0.2f); // Set alpha based on condition
			}
		}
	}
}