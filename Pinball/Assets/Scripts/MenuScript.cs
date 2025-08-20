using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    
    [SerializeField]
    FloatSO score;

    void Start () {
    }
    public void StartGame() {
        score.Value = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
