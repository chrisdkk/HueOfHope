using System;
using System.Collections;
using System.Collections.Generic;
using HandSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIController : MonoBehaviour
{
	[SerializeField] private HandManager handManager;
	[SerializeField] private Button endTurnButton;
	[SerializeField] private TextMeshProUGUI turnIndicator;
	[SerializeField] private Animator playerHealthAnimator;

	private void Start()
	{
		endTurnButton.interactable = false;
		handManager.OnHandDrawn += () => endTurnButton.interactable = true;
		BattleManager.Instance.OnStartPlayerTurn += () => turnIndicator.text = "Player turn";
		BattleManager.Instance.OnStartEnemyTurn += () => turnIndicator.text = "Enemy turn";
		BattleManager.Instance.OnEndBattle += () => endTurnButton.interactable = false;
		BattleManager.Instance.PlayerScript.CharacterStats.OnHealthChange += UpdatePlayerHealthAnimation;
	}

	public void HandleEndTurnButtonClick()
	{
		endTurnButton.interactable = false;
		BattleManager.Instance.EndPlayerTurn();
	}
	
	private static readonly int PlayerHealth = Animator.StringToHash("PlayerHealth");
    private void UpdatePlayerHealthAnimation(int currentHealth, int maxHealth)
    {
        // Set the health parameter in the Animator
        playerHealthAnimator.SetInteger(PlayerHealth, currentHealth);
    }
}
