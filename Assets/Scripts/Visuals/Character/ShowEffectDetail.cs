using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShowEffectDetail : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject effectDetail;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private EffectData effectData;

    void Start()
    {
        title.text = effectData.title;
        description.text = effectData.effectDescription;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        effectDetail.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        effectDetail.SetActive(false);
    }
}