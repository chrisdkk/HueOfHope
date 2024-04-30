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
    IgnoreBlockOnNextAttacks
}
[Serializable]
public class CardEffect
{
    public CardEffectType effectType;
    public GameObject vfxEffect;
    public int payload;
    public bool multipleTargets;
    public bool ignoreBlock;
    public int insightMultiplier;
}
