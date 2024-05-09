using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TogglePauseMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject PauseMenuCanvas;

    private bool canPopUpOpen = false;

    // Start is called before the first frame update
    void Start()
    {
        // temp: disable pop up in game start?
        PauseMenuCanvas.SetActive(false);
        canPopUpOpen = true;
    }

    private void Update()
    {
        // this is probably very inefficient
        // toggle pause menu with ESC
        if (Input.GetKeyDown(KeyCode.Escape) && canPopUpOpen == true)
        {
            PauseMenuCanvas.SetActive(true);
            canPopUpOpen = false;
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && PauseMenuCanvas.activeSelf == true)
        {
            PauseMenuCanvas.SetActive(false);
            canPopUpOpen = true;
        }
    }

    public void OnButtonClick()
    {
        if (canPopUpOpen)
        {
            PauseMenuCanvas.SetActive(true);
            canPopUpOpen = false;
        }
        else
        {
            PauseMenuCanvas.SetActive(false);
            canPopUpOpen = true;
        }
    }
}