using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Chapter", menuName = "ScriptableObjects/Chapter", order = 1)]
public class Chapter : ScriptableObject
{
    public List<Stage> stageList;
    
    // Start is called before the first frame update
    void Start()
    {
        // LoadStageList(stageList); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitializeChapter()
    {
        
    }

    public void LoadStageList(List<Stage> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            list[i].InitializeStage();
        } 
    }
}
