using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RewardCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] public float increasedScale;
    [SerializeField] private GameObject highlightBorder;
    [SerializeField] public LayoutGroup layoutGroup;

    private Vector3 baseScale;
    private CardVisual cardVisual;
    private int siblingIndex;

    public delegate void NonBattleCardClickedEventHandler(RewardCard clickedCard);

    public event NonBattleCardClickedEventHandler OnClick;

    void Start()
    {
        baseScale = transform.localScale;
        highlightBorder.SetActive(false);
        cardVisual = GetComponent<CardVisual>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale *= increasedScale;
        FindObjectOfType<AudioManager>().Play("Hover3");
        cardVisual.ToggleDetails();
        layoutGroup.enabled = false;
        siblingIndex = transform.GetSiblingIndex();
        transform.SetAsLastSibling();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = baseScale;
        cardVisual.ToggleDetails();
        transform.SetSiblingIndex(siblingIndex);
        layoutGroup.enabled = true;
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