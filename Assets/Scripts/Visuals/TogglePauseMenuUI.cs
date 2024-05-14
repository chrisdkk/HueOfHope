using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TogglePauseMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuCanvas;
    [SerializeField] private GameObject confirmationUI;

    public bool isPaused;

    // Start is called before the first frame update
    void Start()
    {
        pauseMenuCanvas.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
            pauseMenuCanvas.SetActive(isPaused);

            Time.timeScale = 0;
        }

        if (!isPaused)
        {
            confirmationUI.SetActive(isPaused);
        }

        if (!isPaused)
        {
            Time.timeScale = 1;
        }
    }

    public void OnButtonClick()
    {
        isPaused = !isPaused;
        pauseMenuCanvas.SetActive(isPaused);
        Time.timeScale = 0;
    }
}