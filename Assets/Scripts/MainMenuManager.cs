using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public int gameSceneIndex = 0;
    public void PlayGame()
    {
        // Loads the next scene in Build Settings
        SceneManager.LoadScene(gameSceneIndex);
    }

    public void QuitGame()
    {
        Debug.Log("Player has quit the game");
        Application.Quit();
    }
}