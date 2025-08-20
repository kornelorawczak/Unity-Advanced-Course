using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void Training() {
        SceneManager.LoadScene(1);
    }

    public void TheRoad() {
        SceneManager.LoadScene(2);
    }

    public void TheOffice() {
        SceneManager.LoadScene(3);
    }

    public void Quit() {
        Application.Quit();
    }
}
