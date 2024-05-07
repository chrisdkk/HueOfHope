using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public void ChangeToPrototypeScene()
    {
        AudioManager audioManager = FindObjectOfType<AudioManager>();
        if (audioManager != null)
        {
            audioManager.Stop("Theme");
        }
        
        FindObjectOfType<AudioManager>().Play("ButtonClick");
        FindObjectOfType<AudioManager>().Play("FirstBattleMusic");
        SceneManager.LoadScene("Prototype");
        
    }

    public void QuitGame()
    {
        FindObjectOfType<AudioManager>().Play("ButtonClick");
        
        #if UNITY_EDITOR
            // This will stop the game in the Unity Editor
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // This will quit the application in a built game
            Application.Quit();
        #endif
    }

    void Start()
    {
        FindObjectOfType<AudioManager>().Play("Theme");
    }
}
