using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerBlockUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<TextMeshProUGUI>().text = "Block: " + GameStateManager.Instance.BattleManager.PlayerStats.defense;
    }
}
