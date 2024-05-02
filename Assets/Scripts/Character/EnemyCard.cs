using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyCardTypes
{
    Attack,
    Block
}

[CreateAssetMenu(fileName = "EnemyCard", menuName = "ScriptableObjects/EnemyCard", order = 1)]
[Serializable]
public class EnemyCard
{
    public EnemyCardTypes cardType = EnemyCardTypes.Attack;
    public List<CardEffect> effects;
}
