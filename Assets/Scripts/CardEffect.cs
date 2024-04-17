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
    BlockDebuff
}

[Serializable]
public class CardEffect
{
    public CardEffectType effectType;
    public int payload;
    public bool multipleTargets;

    /* Apply the effect to the target*/
    public void Apply(Stats user,ref Stats target)
    {
        switch (effectType)
        {
            
            case CardEffectType.Damage:
            {
                int damage = payload+user.insight-user.attackDebuff;
                    int difference = damage - target.defense;
                    
                    if (difference <= 0)
                    {
                        target.defense = Math.Abs(difference);
                        difference = damage = 0;
                    }

                    target.health -= difference;
                    break;
            }

            case CardEffectType.Block:
            {
                target.defense += payload;
                    break;
            }
            
            case CardEffectType.Insight:
            {
                target.insight += payload;
                break;
            }
            
            case CardEffectType.Burn:
            {
                target.burn += payload;
                    break;
            }
            
            case CardEffectType.AttackDebuff:
            {
                target.attackDebuff += payload;
                break;
            }
            
            case CardEffectType.BlockDebuff:
            {
                target.defense -= payload;
                    break;
            }
            
            default: break;
        }
    }
    
}
