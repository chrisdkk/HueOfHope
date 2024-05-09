using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuButtonAction : MonoBehaviour
{
    [SerializeField] private GameObject togglePauseMenuButton;
    
    public void OnResumeButton()
    {
        gameObject.GetComponentInParent<Canvas>().gameObject.SetActive(false);
        togglePauseMenuButton.GetComponent<TogglePauseMenuUI>().canPopUpOpen = true;
    }

    public void OnSaveButton()
    {
    }

    public void OnSettingsButton()
    {
    }

    public void OnQuitMainMenuButton()
    {
        SceneManager.LoadScene("Menu");
    }

    public void OnQuitDesktopButton()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}