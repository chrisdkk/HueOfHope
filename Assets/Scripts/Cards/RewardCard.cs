    using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RewardCard: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,IPointerClickHandler
{
    [SerializeField] public float increasedScale;
    [SerializeField] private GameObject highlightBorder;

    private Vector3 baseScale;

    public delegate void NonBattleCardClickedEventHandler(RewardCard clickedCard);

    public event NonBattleCardClickedEventHandler OnClick;
    
     void Start()
     {
         baseScale = transform.localScale;
         highlightBorder.SetActive(false);
     }

     public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale *= increasedScale;
        FindObjectOfType<AudioManager>().Play("Hover3");
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
        highlightBorder.SetActive(false);
    }

    public void OnRewardChosen()
    {
        highlightBorder.SetActive(true);
        FindObjectOfType<AudioManager>().Play("Hover2");
    }
}