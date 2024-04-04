using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using Random = System.Random;

enum PatternTypes
{
    Attack,
    Block
}

// [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SpawnManagerScriptableObject", order = 1)]
public class EnemyPattern : MonoBehaviour
{
    private PatternTypes types;

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
        // Array PatternActions = PatternTypes.GetValues(typeof(PatternTypes));
        // Random random = new Random();
        // PatternTypes randomPatternAction = (PatternTypes)PatternActions.GetValue(random.Next(PatternActions.Length));
        // Debug.Log(randomPatternAction);

        PatternTypes[] randomArray = RandomizeEnum(PatternTypes.Attack, PatternTypes.Block);
        // Debug.Log(randomArray);

        // // Print the result
        foreach (PatternTypes item in randomArray)
        {
            Debug.Log(item);
        }
    }

    public void GetPattern()
    {
    }

    public void ExecutePatternAction()
    {
        switch (types)
        {
            case PatternTypes.Attack:
                break;
            case PatternTypes.Block:
                break;
            default: break;
        }
    }
}