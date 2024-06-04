using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stage", menuName = "ScriptableObjects/Stage", order = 1)]
public class Stage : ScriptableObject
{
    public string stageName = "Unnamed";
    public Sprite stageBackground;
    public List<GameObject> stageEnemies;
    public string storyText;
}