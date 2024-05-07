using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public static class CardEffectActions
{
    private static AudioManager audioManagerInstance;
    
    public static void SetAudioManager(AudioManager audioManager)
    {
        audioManagerInstance = audioManager;
    }

    public static void DamageAction(Character user, int payload, bool ignoreBlock, ref List<Character> targets)
    {
        int damage = payload + user.CharacterStats.Insight - user.CharacterStats.AttackDebuff;
        // Damage can not be negative
        damage = damage < 0 ? 0 : damage;

        foreach (Character target in targets)
        {
            if (!ignoreBlock)
            {
                int difference = target.CharacterStats.Block - damage;

                if (difference >= 0)
                {
                    target.CharacterStats.Block = difference;
                    difference = 0;
                }
                else
                {
                    target.CharacterStats.Block = 0;
                }

                target.CharacterStats.Health += difference;
                if (audioManagerInstance != null)
                {
                    audioManagerInstance.Play("Attack1");
                }
                
            }
            else
            {
                target.CharacterStats.Health -= damage;
            }

            if (target.CharacterStats.Health < 0)
            {
                target.CharacterStats.Health = 0;
            }
        }
    }

    public static void ShieldBreakAction(Character user, int payload, ref List<Character> targets)
    {
        int damage = payload + user.CharacterStats.Insight - user.CharacterStats.AttackDebuff;
        // Damage can not be negative
        damage = damage < 0 ? 0 : damage;

        foreach (Character target in targets)
        {
            target.CharacterStats.Block = 0;
            target.CharacterStats.Health -= damage;
        }
    }
    
    public static void BlockAction(int payload, ref List<Character> targets)
    {
        foreach (Character target in targets)
        {
            target.CharacterStats.Block += payload;
            if (audioManagerInstance != null)
            {
                audioManagerInstance.Play("Block1");
            }
        }
    }
    
    public static void BurnAction(int payload, ref List<Character> targets)
    {
        foreach (Character target in targets)
        {
            target.CharacterStats.Burn += payload;
            if (audioManagerInstance != null)
            {
                audioManagerInstance.Play("Fire1");
            }
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

    public static void Cleanse(ref List<Character> targets)
    {
        foreach (Character target in targets)
        {
            target.CharacterStats.AttackDebuff = 0;
            target.CharacterStats.Burn = 0;
        }
    }

    public static void InstantApplyBurnAction(ref List<Character> targets)
    {
        foreach (Character target in targets)
        {
            target.CharacterStats.Health -= GameStateManager.Instance.BurnTickDamage * target.CharacterStats.Burn;
            target.CharacterStats.Burn = 0;
        }
    }
    
    public static void TakeOverBurn(Character originalTarget, ref List<Character> targets)
    {
        foreach (Character target in targets)
        {
            target.CharacterStats.Burn += originalTarget.CharacterStats.Burn;
        }
    }
}