using System;
using UnityEngine;
public class Card: MonoBehaviour
{
    public CardData data;
    private bool isSelected = false;
    private CardVisual visual;

    private float originalZIndex;

    public delegate void CardClickedEventHandler(Card clickedCard);

    public event CardClickedEventHandler OnClick;

    private void Start()
    {
        originalZIndex = transform.position.z;
    }

    public void Initialize(CardData cardData)
    {
        data = cardData;
        visual = GetComponentInChildren<CardVisual>();
        visual.Initialize(data);
    }
    
    public void Select()
    {
        BringToFront();
        isSelected = true;
        transform.Translate(Vector3.up * 0.3f);
    }
    
    public void Deselect()
    {
        SendBack();
        isSelected = false;
        transform.Translate(-Vector3.up * 0.3f);
    }
    
    public Stats[] ApplyEffects(Stats[] targets, BattleManager manager)
    {
        foreach (CardEffect effect in data.effects)
        {
            targets = effect.Apply(targets, manager);
        }

        return targets;
    }

    private void OnMouseDown()
    {
        OnClick?.Invoke(this);
    }

    // hover effect
    private void OnMouseEnter()
    {
        if (!isSelected)
        {
            BringToFront();
        }
    }

    private void OnMouseExit()
    {
        if (!isSelected)
        {
            SendBack();
        } 
    }

    private void BringToFront()
    {
        var position = transform.position;
        position = new Vector3(position.x, position.y, 1f);
        transform.position = position;
    }

    private void SendBack()
    {
        var position = transform.position;
        position = new Vector3(position.x, position.y, originalZIndex);
        transform.position = position;
    }
}