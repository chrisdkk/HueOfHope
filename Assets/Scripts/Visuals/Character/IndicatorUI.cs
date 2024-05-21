using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using HandSystem;
using TMPro;
using UnityEngine;

public class IndicatorUI : MonoBehaviour
{
    [SerializeField] private CardMovementController cardMovementController;
    [SerializeField] private GameObject indicatorVisual;
    [SerializeField] private TextMeshProUGUI bubbleText;
    [SerializeField] private float visibleDuration;

    private Sequence sequence;
    
    // Start is called before the first frame update
    void Start()
    {
        cardMovementController.OnTooLowEnergy += TriggerTooLowEnergy;
        indicatorVisual.SetActive(false);
    }

    void TriggerTooLowEnergy()
    {
        if (sequence.IsActive()) return;

        sequence = DOTween.Sequence();

        sequence.AppendCallback(() => indicatorVisual.SetActive(true));

        sequence.AppendInterval(visibleDuration);

        sequence.AppendCallback(() => indicatorVisual.SetActive(false));

        sequence.Play();
    }
}
