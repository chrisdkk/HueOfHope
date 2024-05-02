using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyCardTypes
{
    Attack,
    Block
}

[CreateAssetMenu(fileName = "EnemyCard", menuName = "ScriptableObjects/EnemyCard", order = 1)]
public class EnemyCard : ScriptableObject
{
    public int power = 1; //0-3 determines the strength of the card
    public int tier = 1; // 1-5 determines which enemies uses it
    public EnemyCardTypes cardType = EnemyCardTypes.Attack;
    public List<CardEffect> effects;
}
