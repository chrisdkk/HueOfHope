using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TogglePauseMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuCanvas;
    [SerializeField] private GameObject confirmationUI;

    // Start is called before the first frame update
    void Start()
    {
        pauseMenuCanvas.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnEscapeDown();
        }
    }

    private void OnEscapeDown()
    {
        if (pauseMenuCanvas.activeSelf)
        {
            pauseMenuCanvas.SetActive(false);
            confirmationUI.SetActive(false);
            
            FindObjectOfType<AudioManager>().Play("ButtonClick2");
            BattleManager.Instance.Resume();
            
            return;
        }
        if (!pauseMenuCanvas.activeSelf && !BattleManager.Instance.isPaused)
        {
            BattleManager.Instance.Pause();
            pauseMenuCanvas.SetActive(true);
        }
    }
}