using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapUIController : MonoBehaviour
{
    [SerializeField] private GameObject canvas;
    [SerializeField] private MapSystem mapSystem;
    [SerializeField] private Sprite stageSprite;
    [SerializeField] private Sprite stageCompleteSprite;
    [SerializeField] private Sprite stageActiveSprite;
    [SerializeField] private Sprite stageBossSprite;
    [SerializeField] private TextMeshProUGUI chapterProgress;

    [SerializeField] private Transform stageGroup;
    private Image[] stageImages;

    private void Start()
    {
        mapSystem.OnStageChange += UpdateMapUI;
        stageImages = stageGroup.GetComponentsInChildren<Image>().Where(image => image.name.ToLower().Contains("stage"))
            .ToArray();

        foreach (var image in stageImages)
        {
            image.sprite = stageSprite;
        }

        stageImages[0].sprite = stageActiveSprite;
        chapterProgress.text = "Chapter " + (mapSystem.currentChapterIndex + 1) + "/" + mapSystem.chapterList.Count;
    }

    private void UpdateMapUI(Chapter currentChapter, int stageIndex)
    {
        for (int i = 0; i < stageImages.Length; i++)
        {
            if (i < stageIndex)
            {
                stageImages[i].sprite = stageCompleteSprite;
            }
            else if (i == stageIndex)
            {
                stageImages[i].sprite = stageActiveSprite;
            }
            else
            {
                stageImages[i].sprite = stageSprite;
            }
        }

        chapterProgress.text = "Chapter " + (mapSystem.currentChapterIndex + 1) + "/" + mapSystem.chapterList.Count;
    }
}