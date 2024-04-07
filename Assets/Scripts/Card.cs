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

    public void Initialize(CardData cardData)
    {
        data = cardData;
        visual = GetComponentInChildren<CardVisual>();
        visual.Initialize(data);
    }
    
    public void Select()
    {
        var position = transform.position;
        originalZIndex = position.z;
        position = new Vector3(position.x, position.y, 1f);
        transform.position = position;
        
        isSelected = true;
        visual.SetSelected(true);
    }
    
    public void Deselect()
    {
        var position = transform.position;
        position = new Vector3(position.x, position.y, originalZIndex);
        transform.position = position;
        
        isSelected = false;
        visual.SetSelected(false);
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
}