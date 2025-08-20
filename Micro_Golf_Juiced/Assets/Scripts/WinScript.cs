using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class WinScript : MonoBehaviour
{
    [SerializeField] private Animator result;
    [SerializeField] private Text text;
    [SerializeField] private GameObject continueButton;
    [SerializeField] private ParticleSystem confetti;
    [SerializeField] private Transform ball;
    [SerializeField] private AudioClip level_complete;

    void OnTriggerEnter(Collider other) {
        if (other.tag == "Ball") {
            int tries = BallScript.tries;
            result.Play("WinResult", 0, 0.0f);
            switch(tries) {
                case 1: 
                    text.text = "ACE!";
                    break;
                case 2:
                    text.text = "BIRDIE!";
                    break;
                case 3:
                    text.text = "PAR!";
                    break;
                case 4: 
                    text.text = "BOGEY!";
                    break;
                default:
                    text.text = (tries - 3).ToString() + " - OVER PAR!";
                    break;
            }
            BallScript.isover = true;
            continueButton.SetActive(true);
            confetti.transform.position = ball.position;
            var em = confetti.emission;
            em.enabled = true;
            confetti.Play();
            SoundManager.instance.PlaySoundFX(level_complete, ball, 1f);
            //Invoke("LoadNextScene", 3f);
        }
    }

    public void LoadNextScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        BallScript.tries = 0;
        continueButton.SetActive(false);
        var em = confetti.emission;
        em.enabled = false;
    }
}
