using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndTurnButtonUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnButtonClick()
    {
        FindObjectOfType<AudioManager>().Play("ButtonClick");
        BattleManager.Instance.EndPlayerTurn();

    }
}