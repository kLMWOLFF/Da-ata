using UnityEngine;
using UnityEngine.SceneManagement;
public class TitleScreen : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene("MainDaataScene");
    }

    public void Quit()
    {
        Application.Quit();
        Debug.Log("Game is quitting");
    }
}
