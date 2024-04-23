using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerBlockUI : MonoBehaviour
{
    private void Awake()
    {
        BattleManager.Instance.PlayerScript.CharacterStats.OnStatChange += UpdateBlock;
    }

    // Update is called once per frame
    public void UpdateBlock()
    {
        GetComponent<TextMeshProUGUI>().text = "Block: " + BattleManager.Instance.PlayerScript.CharacterStats.Block;
    }
}
