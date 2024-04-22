using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubscribePlayerUIToStats : MonoBehaviour
{

    public void Subscribe(GameObject Player)
    {
        Player.GetComponent<Player>().CharacterStats.OnStatChange += GameObject.Find("PlayerBlock").GetComponent<PlayerBlockUI>().UpdateBlock;
        Player.GetComponent<Player>().CharacterStats.OnStatChange += GameObject.Find("PlayerHealth").GetComponent<PlayerHealthUI>().UpdateHealthBar;
        Player.GetComponent<Player>().CharacterStats.OnStatChange += Player.GetComponent<UpdateCharacterEffectIndicationsUI>().UpdateBurnIndicator;
        Player.GetComponent<Player>().CharacterStats.OnStatChange += Player.GetComponent<UpdateCharacterEffectIndicationsUI>().UpdateInsightIndicator;
        Player.GetComponent<Player>().CharacterStats.OnStatChange += GameObject.Find("PlayerHealth").GetComponent<PlayerHealthUI>().UpdateHealthBar;
        Player.GetComponent<Player>().OnActionPointChange += GameObject.Find("PlayerActionPoints").GetComponent<PlayerActionPointsUI>().UpdateActionPoints;
        GameStateManager.Instance.BattleManager.OnTurnChange += GameObject.Find("TurnIndication").GetComponent<TurnIndication>().UpdateTurnIndicator;
    }
}
