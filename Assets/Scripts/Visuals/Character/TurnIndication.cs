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
		BattleManager.Instance.OnStartPlayerTurn += () => turnText.text = "Player turn";
		BattleManager.Instance.OnStartEnemyTurn += () => turnText.text = "Enemy turn";
		turnText = GetComponent<TextMeshProUGUI>();
	}
}