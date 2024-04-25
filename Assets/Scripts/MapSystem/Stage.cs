using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stage", menuName = "ScriptableObjects/Stage", order = 1)]
public class Stage : ScriptableObject
{
    public string stageName = "Unnamed";
    public int stageId = 0;
    public Sprite stageBackground;
    public bool hasFightFinished = false;
    public List<Enemy> enemies;
    
    public void InitializeStage()
    {
    }

    public void StartStage()
    {
        Debug.Log("Started Stage With Name: " + stageName);
    }

    public void EndStage()
    {
        hasFightFinished = true;
        Debug.Log("Ended Stage With Name: " + stageName);
    }
}
