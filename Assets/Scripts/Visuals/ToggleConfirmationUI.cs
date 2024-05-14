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
    private QuitType quitType;

    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.Escape))
    //     {
    //         isPaused = !isPaused;
    //         gameObject.SetActive(isPaused);
    //     }
    // }

    public void ShowConfirmationPopUp(QuitType type)
    {
        quitType = type;

        gameObject.SetActive(true);
    }

    public void HideConfirmationPopUp()
    {
        gameObject.SetActive(false);
    }

    public void OnSaveQuit()
    {
        HideConfirmationPopUp();

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
        HideConfirmationPopUp();

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