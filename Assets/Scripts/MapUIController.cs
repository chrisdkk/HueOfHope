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

    [SerializeField] private Transform stageGroup;
    private Image[] stageImages;
    private TextMeshProUGUI[] stageNames;
    
    private void Start()
    {
        mapSystem.OnStageChange += UpdateMapUI;
        stageImages = stageGroup.GetComponentsInChildren<Image>();
        stageNames = stageGroup.GetComponentsInChildren<TextMeshProUGUI>();

        foreach (var image in stageImages)
        {
            image.sprite = stageSprite;
        }

        for (int i = 0; i < stageNames.Length; i++)
        {
            stageNames[i].text = "S -" + (i + 1);
        }
        
        canvas.SetActive(false);
    }

    private void UpdateMapUI(Chapter currentChapter, int stageIndex)
    {
        canvas.SetActive(true);
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
        }
    }
}
