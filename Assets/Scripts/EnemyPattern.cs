using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Timeline;
using Random = System.Random;

public enum EnemyActionTypes
{
    Attack,
    Block
}

public class EnemyPattern
{
    private List<EnemyActionTypes> patternArray;
    private int currentActionIndex = -1;

    /* Set a random pattern consisting of the EnemyActionTypes in the parameter*/
    public EnemyPattern(params EnemyActionTypes[] typesArray)
    {
        int n = typesArray.Length;
        Random random= new Random();
        while (n > 1) 
        {
            int k = random.Next(n--);
            
            EnemyActionTypes temp = typesArray[n];
            typesArray[n] = typesArray[k];
            typesArray[k] = temp;
        }

        patternArray = new List<EnemyActionTypes>(typesArray);
    }

    /* Get the current action */
    public EnemyActionTypes GetCurrentActionPattern()
    {
        currentActionIndex++;
        if (currentActionIndex == patternArray.Count)
        {
            currentActionIndex = 0;
        }

        return patternArray.ElementAt(currentActionIndex);
    }
}