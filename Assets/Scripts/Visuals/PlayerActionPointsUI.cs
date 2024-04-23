using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerActionPointsUI : MonoBehaviour
{
    private void Awake()
    {
        BattleManager.Instance.PlayerScript.OnActionPointChange += UpdateActionPoints;
    }

    // Update is called once per frame
    public void UpdateActionPoints()
    {
        GetComponent<TextMeshProUGUI>().text = "AP: " + BattleManager.Instance.PlayerScript.CurrentActionPoints +
                                               " / " + GameStateManager.Instance.MaxActionPoints;
    }
}