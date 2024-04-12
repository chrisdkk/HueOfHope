using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using Random = System.Random;

public enum PatternTypes
{
    Attack,
    Block
}

// [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SpawnManagerScriptableObject", order = 1)]
public class EnemyPattern
{
    private PatternTypes types;
    private PatternTypes[] randomArray;

    private int currentActionIndex = -1;

    PatternTypes[] RandomizeEnum(PatternTypes type1, PatternTypes type2)
    {
        Random random = new Random();

        // Create an array to hold the enum items
        PatternTypes[] array = new PatternTypes[3];

        // Assign the first two elements randomly
        if (random.Next(2) == 0)
        {
            array[0] = type1;
            array[1] = type2;
        }
        else
        {
            array[0] = type2;
            array[1] = type1;
        }

        // Assign the third element randomly
        array[2] = random.Next(2) == 0 ? type1 : type2;

        return array;
    }

    public void SetPattern()
    {
        // get a random pattern action
        randomArray = RandomizeEnum(PatternTypes.Attack, PatternTypes.Block);
    }

    public PatternTypes GetCurrentActionPattern()
    {
        currentActionIndex++;
        if (currentActionIndex == randomArray.Length)
        {
            currentActionIndex = 0;
        }

        return randomArray[currentActionIndex];
    }
}