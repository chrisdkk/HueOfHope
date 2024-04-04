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

    public void Apply(Stats[] targets, BattleManager manager)
    {
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
                }
                break;
            }
            default: break;
        }
    }
    
}
