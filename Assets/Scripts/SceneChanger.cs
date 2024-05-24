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
        AudioManager audioManager = FindObjectOfType<AudioManager>();
        if (audioManager != null)
        {
            audioManager.Stop("Theme");
            audioManager.Play("ButtonClick");
        }

        GameStateManager.SetGameType(GameType.NewGame);
        SceneManager.LoadScene("Battle");
        StartCoroutine(PlayBattleSound());
    }

    IEnumerator PlayBattleSound()
    {
        while (true)
        {
            if (audioManager != null)
            {
                audioManager.PlayRandomSoundEffect();

                if (isFirstBattleMusicPlaying)
                {
                    audioManager.PlayRandomBackgroundMusic();
                    yield return new WaitForSeconds(audioManager.GetClipLength("FirstBattleMusic"));
                }
                /*else
                {
                    audioManager.Play("SecondBattleMusic");
                    yield return new WaitForSeconds(audioManager.GetClipLength("SecondBattleMusic"));
                }*/
            }

            isFirstBattleMusicPlaying = !isFirstBattleMusicPlaying;
        }
    }


    public void LoadPreviousGame()
    {
        if (PlayerPrefs.HasKey("PlayerHealth") && PlayerPrefs.HasKey("ChapterProgress") &&
            PlayerPrefs.HasKey("StageProgress") && PlayerPrefs.HasKey("PlayerDeck") &&
            PlayerPrefs.HasKey("MaxPlayerHealth") && PlayerPrefs.HasKey("HealingAmount"))
        {
            GameStateManager.SetGameType(GameType.OldGame);
            SceneManager.LoadScene("Battle");
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

    void Start()
    {
        FindObjectOfType<AudioManager>().Play("Theme");

        audioManager = FindObjectOfType<AudioManager>();
        if (audioManager != null)
        {
            audioManager.Play("Theme");
        }

        if (!PlayerPrefs.HasKey("PlayerHealth") && !PlayerPrefs.HasKey("ChapterProgress") &&
            !PlayerPrefs.HasKey("StageProgress") && !PlayerPrefs.HasKey("PlayerDeck"))
        {
            continueButton.SetActive(false);
        }
    }
}