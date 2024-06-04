using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneChanger : MonoBehaviour
{
    private AudioManager audioManager;
    private bool isFirstBattleMusicPlaying = true;
    [SerializeField] private GameObject continueButton;

    public void StartNewGame()
    {
        if (audioManager != null)
        {
            audioManager.Stop("Theme");
            audioManager.Play("ButtonClick");
        }

        GameInitializer.SetGameType(GameType.NewGame);
        SceneManager.LoadScene("Story");
    }

    public void LoadPreviousGame()
    {
        if (audioManager != null)
        {
            audioManager.Stop("Theme");
            audioManager.Play("ButtonClick");
        }

        if (PlayerPrefs.HasKey("PlayerHealth") && PlayerPrefs.HasKey("ChapterProgress") &&
            PlayerPrefs.HasKey("StageProgress") && PlayerPrefs.HasKey("PlayerDeck") &&
            PlayerPrefs.HasKey("MaxPlayerHealth") && PlayerPrefs.HasKey("HealingAmount"))
        {
            GameInitializer.SetGameType(GameType.OldGame);
            SceneManager.LoadScene("Battle");
            audioManager.PlayRandomSoundEffect();
            audioManager.PlayRandomBackgroundMusic();
        }
    }

    public void QuitGame()
    {
        FindObjectOfType<AudioManager>().Play("ButtonClick");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void LoadCredits()
    {
        SceneManager.LoadScene("Credits");
    }

    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        audioManager.StopAllSounds();
        audioManager.Play("Theme");

        if (!PlayerPrefs.HasKey("PlayerHealth") && !PlayerPrefs.HasKey("ChapterProgress") &&
            !PlayerPrefs.HasKey("StageProgress") && !PlayerPrefs.HasKey("PlayerDeck"))
        {
            continueButton.SetActive(false);
        }
    }
}