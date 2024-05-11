using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Card effect types 
public enum CardEffectType
{
    Damage,
    Block,
    Insight,
    Burn,
    AttackDebuff,
    Draw,
    DiscardHand,
    BlockToDamage,
    Cleanse,
    ShieldBreak,
    MultipliedInsightDamage,
    IgnoreBlockOnNextAttacks,
    InstApplyBurn,
    TakeOverBurn
}

public enum CardEffectTarget
{
    Player,
    SingleEnemy,
    MultipleEnemies
}

[Serializable]
public class CardEffect
{
    public static List<CardEffectType> insightAffectedEffects = new List<CardEffectType>()
        { CardEffectType.Damage, CardEffectType.BlockToDamage, CardEffectType.MultipliedInsightDamage };

    public CardEffectType effectType;
    public GameObject vfxEffect;
    public int payload;
    public EffectData effectData;
    public CardEffectTarget effectTarget;
    public bool ignoreBlock;
    public int insightMultiplier;
}