using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Slider = UnityEngine.UI.Slider;

public class VolumeControl : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    private float masterLevel;

    // Start is called before the first frame update
    void Start()
    {
        masterLevel = gameObject.GetComponent<Slider>().value;
    }

    public void SetMasterVolume()
    {
        audioMixer.SetFloat("masterVolume", Mathf.Log10(masterLevel) * 20);
    }
}