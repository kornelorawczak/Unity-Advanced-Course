using UnityEngine;
using UnityEngine.UI;

public class BallScript : MonoBehaviour
{
    [SerializeField] private Transform pivot;
    [SerializeField] private GameObject line;
    [SerializeField, Min(0f)] private float maxForce = 10f;
    [SerializeField, Min(0f)] private float chargeSpeed = 2f;
    [SerializeField] private Text readyText, triesText;
    [SerializeField] private Camera cam, flycam;
    [SerializeField] private ParticleSystem collisionParticle;
    [SerializeField] private Color grassParticle;
    [SerializeField] private Color sandParticle;
    [SerializeField] private AudioClip kick, fall, sand, wood;
    private bool once;
    public static Vector3 lastPosition;
    public static int tries;
    public static bool isover;
    private Rigidbody rb;
    private float currentForce = 0f;
    private bool isCharging = false;
    private float ClampAngle(float angle, float min, float max) {
        if (angle < -180f) angle += 360f;
        if (angle > 180f) angle -= 360f;
        return Mathf.Clamp(angle, min, max);
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        line.transform.position = pivot.transform.position;
        line.transform.rotation = Quaternion.Euler(0, 0, 0);
        line.transform.localScale = new Vector3(1, 1, 0);
        readyText.enabled = true;
        triesText.enabled = true;
        tries = 0;
        triesText.text = "Strokes: " + tries;
        isover = false;
        once = false;
    }

    void Update()
    {
        if (cam.enabled && !isover && !PauseScript.gamePaused) {
            line.transform.position = pivot.transform.position;
            line.transform.rotation = Quaternion.Euler(ClampAngle(pivot.transform.rotation.eulerAngles.x, -50f, 5f), pivot.transform.rotation.eulerAngles.y, 0);
            if (rb.linearVelocity.magnitude < 0.05f) {
                lastPosition = transform.position;
                readyText.enabled = true;
                rb.linearVelocity = Vector3.zero;
                if (Input.GetKeyDown(KeyCode.Space)) {
                    isCharging = true;
                    currentForce = 0f;
                    line.transform.localScale = new Vector3(1, 1, 0);
                }
                if (isCharging && Input.GetKey(KeyCode.Space)) {
                    currentForce += Time.deltaTime * chargeSpeed;
                    currentForce = Mathf.Clamp(currentForce, 0f, maxForce);
                    line.transform.localScale = new Vector3(1, 1, (currentForce * 4f)/ maxForce);
                }
                if (isCharging && Input.GetKeyUp(KeyCode.Space)) {
                    isCharging = false;
                    rb.AddForce(pivot.forward * currentForce);
                    line.transform.localScale = new Vector3(1, 1, 0);
                    tries+=1;
                    once = true;
                    SoundManager.instance.PlaySoundFX(kick, transform, 1f);
                }
            }
            
            else {
                line.transform.localScale = new Vector3(1, 1, 0);
                readyText.enabled = false;
            }
            triesText.text = "Strokes " + tries;
        }
        else if (isover) {
            rb.linearVelocity = Vector3.zero;
            triesText.enabled = false;
            readyText.enabled = false;
        }
        
    }
    private void DisableParticle() {
        var em = collisionParticle.emission;
        em.enabled = false;
    }
    void OnCollisionEnter(Collision col) {
        if (col.gameObject.tag == "Sand") {
            SoundManager.instance.PlaySoundFX(sand, transform, 1f);
            rb.linearVelocity = Vector3.zero;
            if (once) {
                var ma = collisionParticle.main;
                ma.startColor = sandParticle;
                var em = collisionParticle.emission;
                var dur = collisionParticle.main.duration;
                em.enabled = true;
                collisionParticle.Play();
                once = false;
                Invoke(nameof(DisableParticle), dur);
            }
        }
        if (col.gameObject.tag == "Grass") {
            if (once) {
                SoundManager.instance.PlaySoundFX(fall, transform, 1f);
                var ma = collisionParticle.main;
                ma.startColor = grassParticle;
                var em = collisionParticle.emission;
                var dur = collisionParticle.main.duration;
                em.enabled = true;
                collisionParticle.Play();
                once = false;
                Invoke(nameof(DisableParticle), dur);
            }
        }
        else if (col.gameObject.tag != "Ice") {
            SoundManager.instance.PlaySoundFX(wood, transform, 1f);
        }
    }
}
