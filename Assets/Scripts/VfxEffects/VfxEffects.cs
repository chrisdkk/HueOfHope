using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor.Rendering;
using UnityEngine;

public class ParticleSystemProgress : MonoBehaviour
{
    public ParticleSystem particleSystem;
    public float duration = 5f; // Duration of the particle system emission
    private float currentTime = 0f;

    void Start()
    {
        // Start the particle system emission
        particleSystem.Play();

        // Animate the currentTime from 0 to duration using DoTween
        DOTween.To(() => currentTime, x => currentTime = x, duration, duration)
            .OnUpdate(() =>
            {
                // Calculate progress (between 0 and 1) based on currentTime and duration
                float progress = currentTime / duration;

                // Use the progress value as needed, for example:
                Debug.Log("Particle System Progress: " + progress);
            })
            .OnComplete(() =>
            {
                // Particle System emission is complete
                Debug.Log("Particle System Complete!");
            });
    }
}

public class VfxEffects : MonoBehaviour
{
    public static List<CardEffectType> beforeActionVFX = new List<CardEffectType>()
    {
        CardEffectType.Insight, CardEffectType.AttackDebuff, CardEffectType.IgnoreBlockOnNextAttacks,
        CardEffectType.TakeOverBurn
    };

    public static void PlayEffects(GameObject vfx, int payload, params Character[] targets)
    {
        BattleManager.Instance.eventRunning = true;
        bool hasParticle = vfx.GetComponent<ParticleSystem>() != null;

        // Create the effects
        List<GameObject> instVFXs = new List<GameObject>();
        foreach (Character character in targets)
        {
            Vector3 pos = character.transform.Find("Visual").transform.position;
            pos.z = -3;
            if (vfx.name.ToLower().Contains("fire"))
            {
                pos.y -= character.transform.Find("Visual").GetComponent<SpriteRenderer>().bounds.size.y / 2f;
                instVFXs.Add(Instantiate(vfx, pos, Quaternion.identity));
            }
            else
            {
                GameObject instVFX = Instantiate(vfx, pos, Quaternion.identity);
                if (!hasParticle)
                {
                    StatusEffects statusEffects = instVFX.GetComponentInChildren<StatusEffects>();
                    statusEffects.SetText("+" + payload);
                }
                instVFXs.Add(instVFX);
            }
        }

        // Wait for the last effect to finish
        float currentTime = 0f, duration;
        if (hasParticle)
        {
            duration = instVFXs[instVFXs.Count - 1].GetComponent<ParticleSystem>().main.duration;
        }
        else
        {
            duration = instVFXs[instVFXs.Count - 1].GetComponentInChildren<StatusEffects>().duration;
        }
            
        DOTween.To(() => currentTime, x => currentTime = x, duration, duration)
            .OnComplete(() =>
            {
                BattleManager.Instance.eventRunning = false;
                if (!hasParticle)
                {
                    // Destroy all vfx gameobjects
                    foreach (GameObject instVFX in instVFXs)
                    {
                        Destroy(instVFX);
                    }
                }
            });
    }
}