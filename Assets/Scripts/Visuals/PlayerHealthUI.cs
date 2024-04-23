using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering.UI;

public class PlayerHealthUI : MonoBehaviour
{
    void Awake()
    {
        BattleManager.Instance.PlayerScript.CharacterStats.OnStatChange += UpdateHealthBar;
    }
    
    public void UpdateHealthBar()
    {
        GetComponent<TextMeshProUGUI>().text = "HP: " + BattleManager.Instance.PlayerScript.CharacterStats.Health +
                                               " / " + GameStateManager.Instance.maxPlayerHealth;
        GetComponentInChildren<HealthMonitor>().UpdatePlayerAnimation(BattleManager.Instance.PlayerScript.CharacterStats.Health);
    }
}