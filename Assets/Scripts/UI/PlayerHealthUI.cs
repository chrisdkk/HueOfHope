using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerHealthUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<TextMeshProUGUI>().text = "HP: " + GameStateManager.Instance.BattleManager.PlayerStats.health +
                                               " / " + GameStateManager.Instance.maxPlayerHealth;
    }
}