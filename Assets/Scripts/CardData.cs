using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardType
{
    Attack,
    Skill,
    Power
}

[CreateAssetMenu(fileName = "Card", menuName = "ScriptableObjects/Card", order = 1)]
public class CardData : ScriptableObject
{
    public string cardName;
    public int apCost = 0;
    public CardType cardType = CardType.Attack;
    public bool multiTarget;
    public bool targetSelf;
    public CardEffect[] effects;
}
