using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Chapter", menuName = "ScriptableObjects/Chapter", order = 1)]
public class Chapter : ScriptableObject
{
    public List<Stage> stageList;
}