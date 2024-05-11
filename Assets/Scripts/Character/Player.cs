using System;
using System.Collections;
using System.Collections.Generic;
using HandSystem;
using UnityEngine;

public class Player : Character
{
    private int currentActionPoints;
    public int MaxActionPoints;

    void Start()
    {
        CharacterStats.OnHealthChange += CheckForGameOver;
        CharacterStats.OnStatChange += UpdateDamageOfCards;
    }

    public void ResetActionPoints()
    {
        CurrentActionPoints = MaxActionPoints;
    }

    public delegate void ActionPointsChangedEventHandler();

    public event ActionPointsChangedEventHandler OnActionPointChange;

    public int CurrentActionPoints
    {
        get { return currentActionPoints; }
        set
        {
            if (currentActionPoints != value)
            {
                currentActionPoints = value;
                OnActionPointChange?.Invoke();
            }
        }
    }

    private void CheckForGameOver(int currentHealth, int maxHealth)
    {
        if (currentHealth <= 0)
        {
            BattleManager.Instance.EndBattle();
        }
    }
    
    private void UpdateDamageOfCards()
    {
        GameObject.Find("Hand").GetComponent<HandManager>().UpdateCardsInHandVisuals();
    }
}