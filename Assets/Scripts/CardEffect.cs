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
    Discard
}
[Serializable]
public class CardEffect
{
    public CardEffectType effectType;
    public int payload;
    public bool multipleTargets;
    public bool ignoreBlock;
}
