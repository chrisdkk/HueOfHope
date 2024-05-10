using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public void StartNewGame()
    {
        AudioManager audioManager = FindObjectOfType<AudioManager>();
        if (audioManager != null)
        {
            audioManager.Stop("Theme");
        }

        FindObjectOfType<AudioManager>().Play("ButtonClick");
        FindObjectOfType<AudioManager>().Play("FirstBattleMusic");

        GameStateManager.SetGameType(GameType.NewGame);
        SceneManager.LoadScene("Battle");
    }

    public void LoadPreviousGame()
    {
        if (PlayerPrefs.HasKey("PlayerHealth") && PlayerPrefs.HasKey("ChapterProgress") &&
            PlayerPrefs.HasKey("StageProgress") && PlayerPrefs.HasKey("PlayerDeck"))
        {
            GameStateManager.SetGameType(GameType.OldGame);
            SceneManager.LoadScene("Battle");
        }
        else
        {
            // pop up telling you there is no save data
            // hide this button if no previous save data?
            Debug.Log("no savedata");
        }
    }

    public void QuitGame()
    {
        FindObjectOfType<AudioManager>().Play("ButtonClick");

        // add confirmation
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void Start()
    {
        FindObjectOfType<AudioManager>().Play("Theme");
    }
}