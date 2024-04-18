using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubscribePlayerUIToStats : MonoBehaviour
{

    public void Subscribe()
    {
        GameStateManager.Instance.BattleManager.PlayerScript.CharacterStats.OnChange += GameObject.Find("PlayerBlock").GetComponent<PlayerBlockUI>().UpdateBlock;
        GameStateManager.Instance.BattleManager.PlayerScript.CharacterStats.OnChange += GameObject.Find("PlayerHealth").GetComponent<PlayerHealthUI>().UpdateHealthBar;
        GameStateManager.Instance.BattleManager.PlayerScript.OnChange += GameObject.Find("PlayerActionPoints").GetComponent<PlayerActionPointsUI>().UpdateActionPoints;
    }
}
