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

    public void Start()
    {
        masterLevel = gameObject.GetComponent<Slider>().value;
    }

    // public void SetVolume(float volume) {
    public void SetVolume() {
        // audioMixer.SetFloat("MasterVolume", Mathf.Log10(masterLevel) * 20);
        audioMixer.SetFloat("MasterVolume", masterLevel);
    }
}
