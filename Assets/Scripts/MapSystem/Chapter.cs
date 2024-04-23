using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chapter : MonoBehaviour
{
    [SerializeField]
    private List<Stage> StageList = new List<Stage>();
    
    // Start is called before the first frame update
    void Start()
    {
        LoadStageList(StageList); 
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
