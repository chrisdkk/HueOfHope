using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CreditsController : MonoBehaviour
{
    private RectTransform scrollerTransform;
    private float upperScrollLimit = 0;

    // Start is called before the first frame update
    void Start()
    {
        scrollerTransform = GetComponent<RectTransform>();
        DOTween.SetTweensCapacity(500, 50);
    }

    // Update is called once per frame
    void Update()
    {
        if (scrollerTransform.localPosition.y != upperScrollLimit)
        {
            scrollerTransform.DOLocalMoveY(upperScrollLimit, 10, true);
        }
    }
}