using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusEffects : MonoBehaviour
{

    [SerializeField] public float duration;

    [SerializeField] private TextMeshProUGUI textObject;
    [SerializeField] private Image image;
    

    // Start is called before the first frame update
    void Start()
    {
        PlayAnimation();
    }

    private void PlayAnimation()
    {
        image.DOFade(1, duration/500);
        textObject.DOFade(1, duration/500);
        transform.DOMoveY(transform.position.y - 1, duration);
        image.DOFade(0, duration);
        textObject.DOFade(0, duration);
    }

    public void SetText(string text)
    {
        textObject.text = text;
    }
}
