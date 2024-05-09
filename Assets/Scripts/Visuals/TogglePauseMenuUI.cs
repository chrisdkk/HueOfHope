using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TogglePauseMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject PauseMenuCanvas;

    public bool canPopUpOpen = false;

    // Start is called before the first frame update
    void Start()
    {
        // temp: disable pop up in game start?
        PauseMenuCanvas.SetActive(false);
        canPopUpOpen = true;
    }

    private void Update()
    {
        // can this be done in a better way?
        if (Input.GetKeyDown(KeyCode.Escape) && canPopUpOpen)
        {
            PauseMenuCanvas.SetActive(true);
            canPopUpOpen = false;
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && PauseMenuCanvas.activeSelf)
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