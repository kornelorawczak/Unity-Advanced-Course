using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlungerScript : MonoBehaviour
{
    [SerializeField, Min(0f)]
    float power, maxPower = 100f;
    [SerializeField]
    Slider powerSlider;
    List<Rigidbody> ballList;
    bool ready;
    [SerializeField]
    AudioSource src;
    [SerializeField]
    AudioClip sfx_pull, sfx_release;
    void Start()
    {
        powerSlider.minValue = 0f;
        powerSlider.maxValue = maxPower;
        ballList = new List<Rigidbody>();
    }

    void Update()
    {
        if (ready) {
            powerSlider.gameObject.SetActive(true);
        }
        else {
            powerSlider.gameObject.SetActive(false);
        }
        
        powerSlider.value = power;
        if (ballList.Count > 0) {
            ready = true;
            if(Input.GetKeyDown(KeyCode.Space)) {
                src.clip = sfx_pull;
                src.Play();
            }
            if(Input.GetKey(KeyCode.Space)) {
                if (power <= maxPower) {
                    power += 50f * Time.deltaTime;
                }
            }
            if(Input.GetKeyUp(KeyCode.Space)) {
                src.Stop();
                src.clip = sfx_release;
                src.Play();
                foreach(Rigidbody b in ballList) {
                    Debug.Log("Force added");
                    b.AddForce(30 * power * new Vector3(0f,0f,1f));
                }
            }   
        }
        else {
            ready = false;
            power = 0f;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Ball")) {
            ballList.Add(other.gameObject.GetComponent<Rigidbody>());
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("Ball")) {
            ballList.Remove(other.gameObject.GetComponent<Rigidbody>());
            power = 0f;
        }
    }
}
