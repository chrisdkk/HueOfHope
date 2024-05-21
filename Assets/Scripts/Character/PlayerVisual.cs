using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private MapSystem mapSystem;
    
    // Start is called before the first frame update
    void Start()
    {
        mapSystem.OnChapterTwo += EnableColorAnimation;
    }

    public void EnableColorAnimation()
    {
        playerAnimator.SetInteger("ChapterIndex", 2);
    }
}
