using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneChanger : MonoBehaviour
{
    private AudioManager audioManager;
    private bool isFirstBattleMusicPlaying = true;

    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        if (audioManager != null)
        {
            audioManager.Play("Theme");
        }
    }

    public void ChangeToPrototypeScene()
    {
        if (audioManager != null)
        {
            audioManager.Stop("Theme");
            audioManager.Play("ButtonClick");
            audioManager.PlayRandomSoundEffect();
        }

        SceneManager.LoadScene("Prototype");

        // Start playing battle music
        StartCoroutine(PlayBattleMusic());
    }

    IEnumerator PlayBattleMusic()
    {
        while (true)
        {
            if (audioManager != null)
            {
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

    public void QuitGame()
    {
        if (audioManager != null)
        {
            audioManager.Play("ButtonClick");
        }

#if UNITY_EDITOR
        // This will stop the game in the Unity Editor
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // This will quit the application in a built game
        Application.Quit();
#endif
    }
}