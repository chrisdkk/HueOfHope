using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TogglePauseMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject PauseMenuCanvas;

    public bool isPaused;

    // Start is called before the first frame update
    void Start()
    {
        // temp: disable pop up in game start?
        PauseMenuCanvas.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
            PauseMenuCanvas.SetActive(isPaused);
            Time.timeScale = 0;
        }

        if (!isPaused)
        {
            Time.timeScale = 1;
        }
    }

    public void OnButtonClick()
    {
        isPaused = !isPaused;
        PauseMenuCanvas.SetActive(isPaused);
        Time.timeScale = 0;
    }
}