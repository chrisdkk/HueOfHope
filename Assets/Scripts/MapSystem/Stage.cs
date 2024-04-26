using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stage", menuName = "ScriptableObjects/Stage", order = 1)]
public class Stage : ScriptableObject
{
    public string stageName = "Unnamed";
    public Sprite stageBackground;
    public bool hasStageFinished = false;
    public List<Enemy> stageEnemies;
    
    public void InitializeStage()
    {
    }

    public void StartStage()
    {
        Debug.Log("Started Stage With Name: " + stageName);
    }

    public void EndStage()
    {
        Debug.Log("Ended Stage With Name: " + stageName);
    
        hasStageFinished = true;
    }
}