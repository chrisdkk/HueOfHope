using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardType
{
    Attack=0,
    Defense=1,
    Magic=2
}

[CreateAssetMenu(fileName = "Card", menuName = "ScriptableObjects/Card", order = 1)]
public class CardData : ScriptableObject
{
    public string cardName;
    public int apCost = 0;
    public CardType cardType = CardType.Attack;
    public bool multiTarget;
    public List<CardEffect> effects;
    public Texture cardImage;
}
