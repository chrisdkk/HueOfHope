using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class TurnIndication : MonoBehaviour
{
    public void UpdateTurnIndicator(string turnText, bool isEnemyTurn)
    {
        if (!isEnemyTurn)
        {
            Thread.Sleep(500);
        }
        GetComponent<TextMeshProUGUI>().text = turnText;
    }
}
