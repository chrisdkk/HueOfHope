using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapSystem : MonoBehaviour
{
    [SerializeField] public List<Chapter> chapterList;

    [SerializeField] public int currentChapterIndex;
    [SerializeField] public int currentStageIndex;

    [SerializeField] private RewardManager rewardManager;

    private bool chapterOver = false;

    public void InitializeMapSystem()
    {
        rewardManager.OnBattleEnd += EndCurrentStage;

        // default values
        currentChapterIndex = 0;
        currentStageIndex = 0;
    }

    public String GetName()
    {
        return chapterList[currentChapterIndex].stageList[currentStageIndex].stageName;
    }

    public List<GameObject> GetEnemies()
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
        chapterList[currentChapterIndex].stageList[currentStageIndex].EndStage();

        AdvanceToNextStage();

        if (chapterOver == false)
        {
            BattleManager.Instance.Initialize(GameStateManager.Instance.deck,
                chapterList[currentChapterIndex].stageList[currentStageIndex].stageEnemies);
        }
    }

    public void AdvanceToNextStage()
    {
        currentStageIndex++;

        if (currentStageIndex == chapterList[currentChapterIndex].stageList.Count())
        {
            AdvanceToNextChapter();
        }
    }

    public void AdvanceToNextChapter()
    {
        currentChapterIndex++;

        if (currentChapterIndex == chapterList.Count())
        {
            // completed every chapter and stage
            chapterOver = true;
            
            SceneManager.LoadScene("Menu");
        }
    }

    public void ReturnToMap()
    {
    }
}