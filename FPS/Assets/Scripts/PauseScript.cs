using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseScript : MonoBehaviour
{
    public static bool isPaused = false;
    public GameObject pauseMenuUI;
    public GameObject pauseMenuFirst;
    [Header("Player Scripts to deactivate on Pause")]
    public InputManager inputManager;
    public WeaponController wController1;
    public WeaponController wController2;
    void Awake() {
        pauseMenuUI.SetActive(false);
    }

    public void PauseMenuController() {
        if (isPaused) Resume();
        else Pause();
    }

    public void Resume() {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        EventSystem.current.SetSelectedGameObject(null);
        inputManager.enabled = true;
        wController1.enabled = true;
        wController2.enabled = true;
    }

    private void Pause() {
        pauseMenuUI.SetActive(true);
        EventSystem.current.SetSelectedGameObject(pauseMenuFirst);
        Time.timeScale = 0f;
        isPaused = true;
        inputManager.enabled = false;
        wController1.enabled = false;
        wController2.enabled = false;
    }

    public void GoToLevelSelector() {
        SceneManager.LoadScene(0);
        Resume();
    }

    public void QuitGame() {
        Application.Quit();
    }
}
