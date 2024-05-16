using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum QuitType
{
    MainMenu,
    Desktop
}

public class ToggleConfirmationUI : MonoBehaviour
{
    [SerializeField] private GameObject confirmationUI;
    
    private QuitType quitType;
    
    private void Start()
    {
        confirmationUI.SetActive(false);     
    }

    public void ShowConfirmQuitToMenu()
    {
        quitType = QuitType.MainMenu;
        confirmationUI.SetActive(true);
    }
    
    public void ShowConfirmQuitToDesktop()
    {
        quitType = QuitType.Desktop;
        confirmationUI.SetActive(true);
    }
    
    public void OnSaveQuit()
    {
        confirmationUI.SetActive(false);

        if (quitType == QuitType.MainMenu)
        {
            SaveSystem.Instance.SaveGame();
            SceneManager.LoadScene("Menu");
        }
        else if (quitType == QuitType.Desktop)
        {
            SaveSystem.Instance.SaveGame();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
    }

    public void OnOnlyQuit()
    {
        confirmationUI.SetActive(false);

        if (quitType == QuitType.MainMenu)
        {
            SceneManager.LoadScene("Menu");
        }
        else if (quitType == QuitType.Desktop)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
    }
}