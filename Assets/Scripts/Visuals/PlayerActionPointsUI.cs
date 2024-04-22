using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerActionPointsUI : MonoBehaviour
{
    // Update is called once per frame
    public void UpdateActionPoints()
    {
        GetComponent<TextMeshProUGUI>().text = "AP: " + GameStateManager.Instance.BattleManager.PlayerScript.CurrentActionPoints +
                                               " / " + GameStateManager.Instance.MaxActionPoints;
    }
}