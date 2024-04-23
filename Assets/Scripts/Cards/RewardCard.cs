using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RewardCard: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,IPointerClickHandler
{
    [SerializeField] private Vector3 increasedScale;
    [SerializeField] private Sprite hightlightSprite;
    [SerializeField] private Sprite defaultSprite;
    
    private Vector3 baseScale;

    public delegate void NonBattleCardClickedEventHandler(RewardCard clickedCard);

    public event NonBattleCardClickedEventHandler OnClick;
    
     void Start()
     {
         baseScale = transform.localScale;
     }

     public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = increasedScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = baseScale;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick?.Invoke(this);
    }

    public void OnOtherRewardChosen()
    {
        GetComponent<Image>().sprite = defaultSprite;
    }

    public void OnRewardChosen()
    {
        GetComponent<Image>().sprite = hightlightSprite;
    }
}