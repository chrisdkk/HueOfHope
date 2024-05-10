using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuButtonAction : MonoBehaviour
{
    [SerializeField] private GameObject togglePauseMenuButton;

    public void OnResumeButton()
    {
        Time.timeScale = 1;
        gameObject.GetComponentInParent<Canvas>().gameObject.SetActive(false);
        
        togglePauseMenuButton.GetComponent<TogglePauseMenuUI>().isPaused = false;
    }

    public void OnSaveButton()
    {
        SaveSystem.Instance.SaveGame();
    }

    public void OnSettingsButton()
    {
        Debug.Log("Open Settings");
    }

    public void OnQuitMainMenuButton()
    {
        // add confirmation
        
        SceneManager.LoadScene("Menu");
    }

    public void OnQuitDesktopButton()
    {
        // add confirmation
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}