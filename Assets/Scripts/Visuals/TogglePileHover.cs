using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TogglePileHover : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private Animator animator;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        animator.SetTrigger("Play");
    }
}