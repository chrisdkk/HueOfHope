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
    private bool firstEnter = true;

    void Start()
    {
        firstEnter = false;
        title.text = effectData.title;
        description.text =
            effectData.effectDescription.Replace("[NUMBER]",
                GameStateManager.Instance.BurnTickDamage.ToString());
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