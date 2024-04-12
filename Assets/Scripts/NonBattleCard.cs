using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NonBattleCard: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,IPointerClickHandler
{
    [SerializeField] private Vector3 increasedScale;
    [SerializeField] private Sprite hightlightSprite;
    [SerializeField] private Sprite defaultSprite;
    
    public CardData data;
    private CardVisual visual;
    private Vector3 baseScale;

    public delegate void CardClickedEventHandler(NonBattleCard clickedCard);

    public event CardClickedEventHandler OnClick;
    
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