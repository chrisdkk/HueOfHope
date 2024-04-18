using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    private int currentActionPoints;
    public int MaxActionPoints;

    public void ResetActionPoints()
    {
        CurrentActionPoints = MaxActionPoints;
    }
    
    public delegate void ActionPointsChangedEventHandler();

    public event ActionPointsChangedEventHandler OnChange;
    
    public int CurrentActionPoints {
        get { return currentActionPoints; }
        set {
            if (currentActionPoints != value) {
                currentActionPoints = value;
                OnChange?.Invoke();
            }
        }
    }
}
