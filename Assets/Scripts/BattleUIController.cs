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
    [SerializeField] private TextMeshProUGUI playerHealth;

    private void Start()
    {
        endTurnButton.interactable = false;
        handManager.OnHandDrawn += () => endTurnButton.interactable = true;
        BattleManager.Instance.OnStartPlayerTurn += () => turnIndicator.text = "Your Turn";
        BattleManager.Instance.OnStartEnemyTurn += () => turnIndicator.text = "Enemy Turn";
        BattleManager.Instance.OnEndBattle += () => endTurnButton.interactable = false;
        BattleManager.Instance.PlayerScript.CharacterStats.OnHealthChange += UpdatePlayerHealthAnimation;
    }

    public void HandleEndTurnButtonClick()
    {
        endTurnButton.interactable = false;
        BattleManager.Instance.EndPlayerTurn();
    }

    private void UpdatePlayerHealthAnimation(int currentHealth, int maxHealth)
    {
        // Set the health parameter in the Animator
        playerHealth.text = "HP: " + currentHealth + "/" + maxHealth;
    }
}