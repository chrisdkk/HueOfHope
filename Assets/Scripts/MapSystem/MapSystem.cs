using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapSystem : MonoBehaviour
{
    [SerializeField] public List<Chapter> chapterList;

    [SerializeField] public int currentChapterIndex;
    [SerializeField] public int currentStageIndex;

    [SerializeField] private RewardManager rewardManager;

    public event Action OnChapterChange;

    private bool chapterOver = false;

    public event Action<Chapter, int> OnStageChange;

    public void InitializeMapSystem()
    {
        rewardManager.OnBattleEnd += EndCurrentStage;
    }

    public List<GameObject> GetEnemies()
    {
        return chapterList[currentChapterIndex].stageList[currentStageIndex].stageEnemies;
    }

    public Sprite GetBackground()
    {
        return chapterList[currentChapterIndex].stageList[currentStageIndex].stageBackground;
    }

    public string GetStory()
    {
        return chapterList[currentChapterIndex].stageList[currentStageIndex].storyText;
    }

    private void EndCurrentStage()
    {
        AdvanceToNextStage();

        if (chapterOver == false)
        {
            OnStageChange?.Invoke(chapterList[currentChapterIndex], currentStageIndex);
        }
    }

    public void StartNextStage()
    {
        Stage currentStage = chapterList[currentChapterIndex].stageList[currentStageIndex];
        BattleManager.Instance.Initialize(GameInitializer.Instance.deck,
            currentStage.stageEnemies,
            currentStage.stageBackground,
            currentStage.storyText);
    }

    private void AdvanceToNextStage()
    {
        currentStageIndex++;

        if (currentStageIndex == chapterList[currentChapterIndex].stageList.Count())
        {
            AdvanceToNextChapter();
        }
        else
        {
            StartNextStage();
        }
    }

    private void AdvanceToNextChapter()
    {
        currentChapterIndex++;
        currentStageIndex = 0;

        if (currentChapterIndex == chapterList.Count())
        {
            // completed every chapter and stage
            chapterOver = true;
            SceneManager.LoadScene("Win");
        }
        else
        {
            // Switch for improvements the player gets after a chapter
            switch (currentChapterIndex)
            {
                case 1:
                    GameInitializer.Instance.maxPlayerHealth += 15;
                    GameInitializer.Instance.HealingAmount += 5;
                    break;
                case 2:
                    GameInitializer.Instance.maxPlayerHealth += 15;
                    GameInitializer.Instance.HealingAmount += 5;
                    break;
            }

            GameInitializer.Instance.CurrentPlayerHealth = GameInitializer.Instance.maxPlayerHealth;
            OnChapterChange?.Invoke();
            OnStageChange?.Invoke(chapterList[currentChapterIndex], currentStageIndex);
            StartNextStage();
        }
    }
}