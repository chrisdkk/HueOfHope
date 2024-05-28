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
        BattleManager.Instance.Initialize(GameStateManager.Instance.deck,
            currentStage.stageEnemies,
            currentStage.stageBackground,
            currentStage.storyText);
    }

    private void AdvanceToNextStage()
    {
        currentStageIndex++;
        
        if (currentStageIndex != chapterList[currentChapterIndex].stageList.Count())
        {
            StartNextStage();
        }

        if (currentStageIndex == chapterList[currentChapterIndex].stageList.Count())
        {
            AdvanceToNextChapter();
        }
    }

    private void AdvanceToNextChapter()
    {
        currentChapterIndex++;
        currentStageIndex = 0;
        OnChapterChange?.Invoke();

        // Switch for improvements the player gets after a chapter
        switch (currentChapterIndex)
        {
            case 1:
                GameStateManager.Instance.maxPlayerHealth += 15;
                GameStateManager.Instance.HealingAmount += 5;
                break;
            case 2:
                GameStateManager.Instance.maxPlayerHealth += 15;
                GameStateManager.Instance.HealingAmount += 5;
                break;
        }

        GameStateManager.Instance.CurrentPlayerHealth = GameStateManager.Instance.maxPlayerHealth;

        if (currentChapterIndex == chapterList.Count())
        {
            // completed every chapter and stage
            chapterOver = true;

            SceneManager.LoadScene("Win");
        }
    }
}