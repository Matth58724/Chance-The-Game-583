using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void PlayGame()
    {
        // Loads the next scene in Build Settings
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Debug.Log("Player has quit the game");
        Application.Quit();
    }
}