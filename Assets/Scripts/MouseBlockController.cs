using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseBlockController : MonoBehaviour
{
    [SerializeField] private GameObject mouseBlocker;

    private void Start()
    {
        mouseBlocker.SetActive(false);
    }

    public void ToggleMouseBlocker()
    {
        mouseBlocker.SetActive(!mouseBlocker.activeSelf);
    }
}
