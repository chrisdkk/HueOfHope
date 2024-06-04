using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    public AudioMixer audioMixer;
    private float masterLevel;

    public void SetVolume()
    {
        masterLevel = gameObject.GetComponent<Slider>().value;

        audioMixer.SetFloat("MasterVolume", Mathf.Log10(masterLevel) * 20);
    }
}