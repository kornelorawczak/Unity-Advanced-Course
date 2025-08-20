using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BallScript : MonoBehaviour
{
    [SerializeField]
    Text scoreText, multText;
    [SerializeField]
    FloatSO scoreSO;
    float mult = 1.0f;
    [SerializeField]
    Transform Ball;
    [SerializeField]
    AudioSource src;
    [SerializeField]
    AudioClip sfx_bouncer, sfx_end, sfx_wall;
    void Start()
    {
        scoreText.text = "Score: " + scoreSO.Value;
        multText.text = "Multiplier = " + mult + "x";
    }

    void Update()
    {
        if (Ball.position.z < -3) {
            src.clip = sfx_end;
            src.Play();
        }
        if (Ball.position.z < -10) {
            EndGame();
        }
    }

    private void OnCollisionEnter(Collision other) {
        if (other.collider.tag == "Bouncer") {
            src.clip = sfx_bouncer;
            src.Play();
            scoreSO.Value = Mathf.RoundToInt(scoreSO.Value + 250.0f * mult);
            scoreText.text = "Score: " + scoreSO.Value;
            mult += 0.2f;
            multText.text = "Multiplier = " + mult + "x";
        }
        if (other.collider.tag == "Walls") {
            src.clip = sfx_wall;
            src.Play();
        }
    }

    private void EndGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
