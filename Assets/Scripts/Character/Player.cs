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
    }

    public void ResetActionPoints()
    {
        CurrentActionPoints = MaxActionPoints;
    }

    public void ResetBuffsAndDebuffs()
    {
        CharacterStats.Block = 0;
        CharacterStats.AttackDebuff = 0;
        CharacterStats.IgnoreBlockOnNext = 0;
        CharacterStats.Insight = 0;
        CharacterStats.Burn = 0;
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
}