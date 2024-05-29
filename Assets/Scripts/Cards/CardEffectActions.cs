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

    public static void DamageAction(GameObject vfxEffect, Character user, int payload, bool ignoreBlock,
        ref List<Character> targets)
    {
        int damage = payload != 0 ? payload + user.CharacterStats.Insight - user.CharacterStats.AttackDebuff : 0;
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

        VfxEffects.PlayEffects(vfxEffect, damage, targets.ToArray());
        VfxEffects.PlayEffects(BattleManager.Instance.dmbNumberEffect, damage, targets.ToArray());
    }

    public static void ShieldBreakAction(GameObject vfxEffect, Character user, int payload, ref List<Character> targets)
    {
        int damage = payload != 0 ? payload + user.CharacterStats.Insight - user.CharacterStats.AttackDebuff : 0;
        // Damage can not be negative
        damage = damage < 0 ? 0 : damage;

        foreach (Character target in targets)
        {
            target.CharacterStats.Block = 0;
            target.CharacterStats.Health -= damage;
        }

        VfxEffects.PlayEffects(vfxEffect, damage, targets.ToArray());
    }

    public static void BlockAction(GameObject vfxEffect, int payload, ref List<Character> targets)
    {
        foreach (Character target in targets)
        {
            target.CharacterStats.Block += payload;
            if (audioManagerInstance != null)
            {
                audioManagerInstance.Play("Block1");
            }
        }

        VfxEffects.PlayEffects(vfxEffect, payload, targets.ToArray());
        VfxEffects.PlayEffects(BattleManager.Instance.blockNumberEffect, payload, targets.ToArray());
    }

    public static void BurnAction(GameObject vfxEffect, int payload, ref List<Character> targets)
    {
        foreach (Character target in targets)
        {
            target.CharacterStats.Burn += payload;
            if (audioManagerInstance != null)
            {
                audioManagerInstance.Play("Fire1");
            }
        }

        VfxEffects.PlayEffects(vfxEffect, payload, targets.ToArray());
    }

    public static void InsightAction(GameObject vfxEffect, int payload, ref List<Character> targets)
    {
        foreach (Character target in targets)
        {
            target.CharacterStats.Insight += payload;
        }

        VfxEffects.PlayEffects(vfxEffect, payload, targets.ToArray());
    }

    public static void AttackDebuff(GameObject vfxEffect, int payload, ref List<Character> targets)
    {
        foreach (Character target in targets)
        {
            target.CharacterStats.AttackDebuff += payload;
        }

        VfxEffects.PlayEffects(vfxEffect, payload, targets.ToArray());
    }

    public static void Cleanse(GameObject vfxEffect, ref List<Character> targets)
    {
        foreach (Character target in targets)
        {
            target.CharacterStats.AttackDebuff = 0;
            target.CharacterStats.Burn = 0;
        }

        VfxEffects.PlayEffects(vfxEffect, 0, targets.ToArray());
    }

    public static void BurnMultipliedByAPAction(GameObject vfxEffect, ref List<Character> targets)
    {
        foreach (Character target in targets)
        {
            target.CharacterStats.Health -= target.CharacterStats.Burn;
            if (audioManagerInstance != null)
            {
                audioManagerInstance.Play("Fire1");
            }

            VfxEffects.PlayEffects(vfxEffect, target.CharacterStats.Burn, targets.ToArray());
            VfxEffects.PlayEffects(BattleManager.Instance.dmbNumberEffect, target.CharacterStats.Burn,
                targets.ToArray());
        }
    }

    public static void TakeOverBurn(GameObject vfxEffect, Character originalTarget, ref List<Character> targets)
    {
        foreach (Character target in targets)
        {
            target.CharacterStats.Burn += originalTarget.CharacterStats.Burn;
            if (audioManagerInstance != null)
            {
                audioManagerInstance.Play("Fire1");
            }
        }

        VfxEffects.PlayEffects(vfxEffect, originalTarget.CharacterStats.Burn, targets.ToArray());
    }
}