using UnityEngine;

public class SpeedBoostScript : MonoBehaviour
{

    [SerializeField, Min(0)]
    float speedBoostForce = 20f;
    [SerializeField]
    AudioSource src;
    [SerializeField]
    AudioClip sfx_swoosh;
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Ball")) {
            Rigidbody ballRB = other.GetComponent<Rigidbody>();
            if (ballRB != null && ballRB.linearVelocity.z > 0) {
                //Debug.Log("Ball boosted");
                src.clip = sfx_swoosh;
                src.Play();
                ballRB.linearVelocity += ballRB.linearVelocity.normalized * speedBoostForce;
            }
        }
    }
}
