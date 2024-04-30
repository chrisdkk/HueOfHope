using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public void ChangeToPrototypeScene()
    {
        SceneManager.LoadScene("Prototype");
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            // This will stop the game in the Unity Editor
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // This will quit the application in a built game
            Application.Quit();
        #endif
    }
}
