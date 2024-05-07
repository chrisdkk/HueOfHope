using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VfxEffects : MonoBehaviour
{

    public static IEnumerator PlayEffects(GameObject vfx, params Character[] targets)
    {
        BattleManager.Instance.eventRunning = true;

        // Create the effects
        List<GameObject> instVFXs = new List<GameObject>();
        foreach (Character character in targets)
        {
            GameObject instGameObject = Instantiate(vfx, character.transform.Find("Visual").transform.position, Quaternion.identity);
            AdaptColor(instGameObject);
            instVFXs.Add(instGameObject);
        }

        // Wait for the last effect to finish
        yield return new WaitForSeconds(instVFXs[instVFXs.Count-1].GetComponent<ParticleSystem>().main.duration);
        // Destroy all vfx gameobjects
        foreach (GameObject instVFX in instVFXs)
        {
            Destroy(instVFX);
        }

        BattleManager.Instance.eventRunning = false;
    }

    private static void AdaptColor(GameObject vfx)
    {
        float r, g, b;
        
        ParticleSystem.MainModule setting = vfx.GetComponent<ParticleSystem>().main;
        r = setting.startColor.color.r;
        g = setting.startColor.color.g;
        b = setting.startColor.color.b;
        if (!GameStateManager.Instance.redEnabled)
        {
            r = r<=127? 0:255;
        }
        if (!GameStateManager.Instance.greenEnabled)
        {
            g = g<=127? 0:255;
        }
        if (!GameStateManager.Instance.blueEnabled)
        {
            b = b<=127? 0:255;
        }
        setting.startColor = new Color(r, g, b, 0.75f);

        // Change color of lifetime
        ParticleSystem.ColorOverLifetimeModule colorOverLifetimeModule = vfx.GetComponent<ParticleSystem>().colorOverLifetime;
        ParticleSystem.MinMaxGradient minMaxGradient = colorOverLifetimeModule.color;

        // Max
        r = minMaxGradient.color.r;
        g = minMaxGradient.color.g;
        b = minMaxGradient.color.b;
        if (!GameStateManager.Instance.redEnabled)
        {
            r = r<=127? 0:255;
        }
        if (!GameStateManager.Instance.greenEnabled)
        {
            g = g<=127? 0:255;
        }
        if (!GameStateManager.Instance.blueEnabled)
        {
            b = b<=127? 0:255;
        }
        minMaxGradient.color = new Color(r,g,b, 0.75f);
        colorOverLifetimeModule.color = minMaxGradient;
        
        foreach (ParticleSystem particleSystem in vfx.GetComponentsInChildren<ParticleSystem>())
        {
            setting = particleSystem.main;
            r = setting.startColor.color.r;
            g = setting.startColor.color.g;
            b = setting.startColor.color.b;
            if (!GameStateManager.Instance.redEnabled)
            {
                r = r<=127? 0:255;
            }
            if (!GameStateManager.Instance.greenEnabled)
            {
                g = g<=127? 0:255;
            }
            if (!GameStateManager.Instance.blueEnabled)
            {
                b = b<=127? 0:255;
            }
            setting.startColor = new Color(r, g, b, 0.75f);
            
            // Change color of lifetime
            colorOverLifetimeModule = vfx.GetComponent<ParticleSystem>().colorOverLifetime;
            minMaxGradient = colorOverLifetimeModule.color;

            // Max
            r = minMaxGradient.color.r;
            g = minMaxGradient.color.g;
            b = minMaxGradient.color.b;
            if (!GameStateManager.Instance.redEnabled)
            {
                r = r<=127? 0:255;
            }
            if (!GameStateManager.Instance.greenEnabled)
            {
                g = g<=127? 0:255;
            }
            if (!GameStateManager.Instance.blueEnabled)
            {
                b = b<=127? 0:255;
            }
            minMaxGradient.color = new Color(r,g,b, 0.75f);
            colorOverLifetimeModule.color = minMaxGradient;
        }
    }
}
