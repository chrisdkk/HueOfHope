using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubscribePlayerUIToStats : MonoBehaviour
{

    public void Subscribe()
    {
        GameStateManager.Instance.BattleManager.PlayerScript.CharacterStats.OnStatChange += GameObject.Find("PlayerBlock").GetComponent<PlayerBlockUI>().UpdateBlock;
        GameStateManager.Instance.BattleManager.PlayerScript.CharacterStats.OnStatChange += GameObject.Find("PlayerHealth").GetComponent<PlayerHealthUI>().UpdateHealthBar;
        GameStateManager.Instance.BattleManager.PlayerScript.OnActionPointChange += GameObject.Find("PlayerActionPoints").GetComponent<PlayerActionPointsUI>().UpdateActionPoints;
        GameStateManager.Instance.BattleManager.OnTurnChange += GameObject.Find("TurnIndication").GetComponent<TurnIndication>().UpdateTurnIndicator;
    }
}
