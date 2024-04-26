using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapSystem : MonoBehaviour
{
    [SerializeField] public List<Chapter> chapterList;

    [SerializeField] public int currentChapterIndex;
    [SerializeField] public int currentStageIndex;

    [SerializeField] private RewardManager rewardManager;

    public void InitializeMapSystem()
    {
        // THIS FUNCTION MIGHT NOT BE NECESSARY
        Debug.Log("Map-System Initialized");

        rewardManager.OnBattleEnd += EndCurrentStage;

        // default values
        currentChapterIndex = 0;
        currentStageIndex = 0;
    }

    public String GetName()
    {
        return chapterList[currentChapterIndex].stageList[currentStageIndex].stageName;
    }

    public List<Enemy> GetEnemies()
    {
        return chapterList[currentChapterIndex].stageList[currentStageIndex].stageEnemies;
    }

    public Sprite GetBackground()
    {
        return chapterList[currentChapterIndex].stageList[currentStageIndex].stageBackground;
    }

    public void LoadStage()
    {
        chapterList[currentChapterIndex].stageList[currentStageIndex].StartStage();
    }

    public void EndCurrentStage()
    {
        // ReturnToMap();
        chapterList[currentChapterIndex].stageList[currentStageIndex].EndStage();
        
        AdvanceToNextStage();
        GameStateManager.Instance.StartBattle();

        // // advance one stage forward
        // currentStageIndex++;
        //
        // if (currentStageIndex == chapterList[currentChapterIndex].stageList.Count())
        // {
        //     // advance one chapter forward
        //     currentChapterIndex++;
        //
        //     if (currentChapterIndex == chapterList.Count())
        //     {
        //         // completed every chapter and stage
        //     }
        // }
    }

    public void AdvanceToNextStage()
    {
        // advance one stage forward
        currentStageIndex++;

        if (currentStageIndex == chapterList[currentChapterIndex].stageList.Count())
        {
            AdvanceToNextChapter();
        }
    }

    public void AdvanceToNextChapter()
    {
        // advance one chapter forward
        currentChapterIndex++;

        if (currentChapterIndex == chapterList.Count())
        {
            // completed every chapter and stage
        }
    }

    public void ReturnToMap()
    {
        Debug.Log("hioio");
        Debug.Log("ddfdffff");
    }
}