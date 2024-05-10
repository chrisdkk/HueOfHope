using System;
using System.Collections;
using System.Collections.Generic;
using HandSystem;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIController : MonoBehaviour
{
	[SerializeField] private HandManager handManager;
	[SerializeField] private Button endTurnButton;

	private void Start()
	{
		endTurnButton.interactable = false;
		handManager.OnHandDrawn += () =>
		{
			endTurnButton.interactable = true;
		};
	}

	public void HandleEndTurnButtonClick()
	{
		endTurnButton.interactable = false;
		BattleManager.Instance.EndPlayerTurn();
	}
}
