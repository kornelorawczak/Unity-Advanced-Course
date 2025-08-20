using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class PauseScript : MonoBehaviour
{
    public static bool gamePaused = false;
    [SerializeField] GameObject pauseMenuUI;
    [SerializeField] Camera flycam;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (!gamePaused) {
                Pause();    
            }
        }
    }

    public void Resume() {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        gamePaused = false;
        if(flycam.enabled) {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void Pause() {
        pauseMenuUI.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;
        gamePaused = true;
    }
}
