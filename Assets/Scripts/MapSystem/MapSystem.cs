using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSystem : MonoBehaviour
{
    [SerializeField] private List<Chapter> chapterList;
    
    // Start is called before the first frame update
    void Start()
    {
        InitializeMapSystem(); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitializeMapSystem()
    {
        foreach (Chapter chapter in chapterList)
        {
            foreach (Stage stage in chapter.stageList)
            {
                // for now, this start every stage in the stage list
                // ideally what you want is that one stage starts, and only after it has ended, the next one starts
                stage.StartStage();
            }
        } 
    }
}
