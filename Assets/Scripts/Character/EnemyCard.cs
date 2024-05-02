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
    public EnemyCardTypes cardType = EnemyCardTypes.Attack;
    public List<CardEffect> effects;
}
