using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerHealthUI : MonoBehaviour
{
    public void UpdateHealthBar()
    {
        GetComponent<TextMeshProUGUI>().text = "HP: " + GameStateManager.Instance.BattleManager.PlayerScript.CharacterStats.Health +
                                               " / " + GameStateManager.Instance.maxPlayerHealth;
    }
}