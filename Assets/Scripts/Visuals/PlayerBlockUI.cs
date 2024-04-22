using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerBlockUI : MonoBehaviour
{
    // Update is called once per frame
    public void UpdateBlock()
    {
        GetComponent<TextMeshProUGUI>().text = "Block: " + GameStateManager.Instance.BattleManager.PlayerScript.CharacterStats.Block;
    }
}
