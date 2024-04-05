using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayCardEvent : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMouseDown()
    {
        if (GameStateManager.Instance.BattleManager.CurrentActionPoints >= GetComponent<CardVisual>().card.apCost)
        {
            // if (GetComponent<CardVisual>().card.cardType == CardType.Attack)
            // {
                GameStateManager.Instance.BattleManager.ApplyCardEffect(transform.parent.gameObject, GameStateManager.Instance.BattleManager.EnemiesInBattle[0]);
                
            // }
            
        }
        else
        {
            Debug.Log("Not enough AP");
        }
    }
}
