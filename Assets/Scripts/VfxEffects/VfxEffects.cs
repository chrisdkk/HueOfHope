using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor.Rendering;
using UnityEngine;

public class VfxEffects : MonoBehaviour
{
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
                FindObjectOfType<AudioManager>().Play("Fire1");
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

        DOTween.To(() => currentTime, x => currentTime = x, duration / 2, duration / 2)
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