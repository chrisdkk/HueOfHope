using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Card effect types 
public enum CardEffectType
{
    Damage,
    Defend,
}

[Serializable]
public class CardEffect
{
    public CardEffectType effectType;
    public int[] payload;

    public Stats[] Apply(Stats[] targets, BattleManager manager)
    {
        Debug.Log(effectType + "-" + targets[0] + "-" + payload[0]);
        switch (effectType)
        {
            case CardEffectType.Damage:
            {
                for (int i = 0; i < targets.Length; i++)
                {
                    Stats target = targets[i];
                    int damage = payload[0];
                    int difference = damage - target.defense;
                    
                    if (difference < 0)
                    {
                        damage = 0;
                        target.defense = Math.Abs(difference);
                    }

                    target.health -= damage;
                    targets[i] = target;
                }
                break;
            }
            case CardEffectType.Defend:
            {
                for (int i = 0; i < targets.Length; i++)
                {
                    Stats target = targets[i];
                    int defense = payload[0];

                    target.defense += defense;
                    targets[i] = target;
                }
                break;
            }
            default: break;
        }
        
        return targets;
    }
    
}
