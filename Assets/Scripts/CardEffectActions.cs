using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public static class CardEffectActions
{

    public static void DamageAction(Character user, int payload, bool ignoreBlock, ref List<Character> targets)
    {

        int damage = payload+user.CharacterStats.Insight-user.CharacterStats.AttackDebuff;
        // Damage can not be negative
        damage = damage < 0 ? 0 : damage;

        foreach (Character target in targets)
        {
            if (!ignoreBlock)
            {
                int difference = damage - target.CharacterStats.Block;
                    
                if (difference <= 0)
                {
                    target.CharacterStats.Block=Math.Abs(difference);
                    difference = 0;
                }
                target.CharacterStats.Health -= difference;
                target.CharacterStats.Block=0;
            }
            else
            {
                target.CharacterStats.Health -= damage;
            }
        }
    }
    
    public static void BlockAction(int payload, ref List<Character> targets)
    {
        foreach (Character target in targets)
        {
            target.CharacterStats.Block += payload;
        }
    }
    
    public static void BurnAction(int payload, ref List<Character> targets)
    {
        foreach (Character target in targets)
        {
            target.CharacterStats.Burn += payload;
        }
    }
    
    public static void InsightAction(int payload, ref List<Character> targets)
    {
        foreach (Character target in targets)
        {
            target.CharacterStats.Insight += payload;
        }
    }
    
    public static void AttackDebuff(int payload, ref List<Character> targets)
    {
        foreach (Character target in targets)
        {
            target.CharacterStats.AttackDebuff += payload;
        }
    }
}
