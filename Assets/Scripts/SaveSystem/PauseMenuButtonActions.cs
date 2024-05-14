using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuButtonActions : MonoBehaviour
{
    [SerializeField] private GameObject togglePauseMenuButton;
    [SerializeField] private GameObject confirmationUI;

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

    public void OnQuitMainMenuButton()
    {
        confirmationUI.GetComponent<ToggleConfirmationUI>().ShowConfirmationPopUp(QuitType.MainMenu);
    }

    public void OnQuitDesktopButton()
    {
        confirmationUI.GetComponent<ToggleConfirmationUI>().ShowConfirmationPopUp(QuitType.Desktop);
    }
}