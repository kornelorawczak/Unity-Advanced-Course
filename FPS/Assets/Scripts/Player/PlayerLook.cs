using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public Camera cam;
    public Animator animator;
    private float xRotation = 0f;
    public float xSensitivy = 30f;
    public float ySensitivy = 30f;
    public float aimingSensivityEffector;
    private void Start() {
        cam.transform.rotation = Quaternion.Euler(0,0,0);
    }
    
    public void ProcessLook(Vector2 input) {
        if (animator.GetBool("IsDead")) {
            return;
        }
        float mouseX = input.x;
        float mouseY = input.y;

        xRotation -= (mouseY * Time.deltaTime) * (PlayerMotor.isAiming ? ySensitivy * aimingSensivityEffector : ySensitivy);
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        cam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        transform.Rotate(Vector3.up * (mouseX * Time.deltaTime) * (PlayerMotor.isAiming ? xSensitivy * aimingSensivityEffector : xSensitivy));
    }
}
