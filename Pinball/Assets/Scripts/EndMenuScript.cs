using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndMenuScript : MonoBehaviour
{
    [SerializeField]
    FloatSO score;
    [SerializeField]
    Text endScoreText;
    void Start () {
        endScoreText.text = "Score: " + score.Value;
    }
    public void RestartGame() { 
        score.Value = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void ExitGame() {
        Application.Quit();
    }
}
