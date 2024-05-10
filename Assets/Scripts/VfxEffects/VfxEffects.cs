using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class VfxEffects : MonoBehaviour
{
    public static List<CardEffectType> beforeActionVFX = new List<CardEffectType>()
    {
        CardEffectType.Insight, CardEffectType.AttackDebuff, CardEffectType.IgnoreBlockOnNextAttacks,
        CardEffectType.TakeOverBurn
    };

    public static IEnumerator PlayEffects(GameObject vfx, int payload, params Character[] targets)
    {
        BattleManager.Instance.eventRunning = true;

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
                instVFXs.Add(Instantiate(vfx, pos, Quaternion.identity));
            }
        }

        // Wait for the last effect to finish
        if (instVFXs[instVFXs.Count - 1].GetComponent<ParticleSystem>() != null)
        {
            yield return new WaitForSeconds(instVFXs[instVFXs.Count - 1].GetComponent<ParticleSystem>().main.duration);
        }
        else
        {
            StatusEffects statusEffects = instVFXs[instVFXs.Count - 1].GetComponentInChildren<StatusEffects>();
            statusEffects.SetText("+" + payload);
            yield return new WaitForSeconds(instVFXs[instVFXs.Count - 1].GetComponentInChildren<StatusEffects>()
                .duration);
        }


        // Destroy all vfx gameobjects
        foreach (GameObject instVFX in instVFXs)
        {
            Destroy(instVFX);
        }
        BattleManager.Instance.eventRunning = false;
    }
}