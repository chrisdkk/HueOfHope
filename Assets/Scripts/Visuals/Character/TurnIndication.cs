using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class TurnIndication : MonoBehaviour
{
	private TextMeshProUGUI turnText;
	
	private void Start()
	{
		BattleManager.Instance.OnTurnChange += UpdateTurnIndicator;
		turnText = GetComponent<TextMeshProUGUI>();
	}

	private void UpdateTurnIndicator(bool isEnemyTurn)
	{
		turnText.text = isEnemyTurn ? "Enemy Turn" : "Player Turn";
	}
}