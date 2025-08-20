using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class CameraMovementScript : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private Camera flycam;
    [SerializeField] private Transform target, pivot;
    [SerializeField] private Vector3 offset;
    [SerializeField] private GameObject ball;
    [SerializeField] private bool useOffsetValues;
    [SerializeField] private float scrollSpeed = 10f;
    [SerializeField, Min(0f)] private float rotateSpeed, flycam_normal_speed, flycam_sprint_speed, flycam_sensitivity, flycam_current_speed;
    private Quaternion currentRotation;
    private Quaternion lastRotation;
    private Rigidbody rb;
    private Vector3 lastMouse = new Vector3(255, 255, 255);
    public Transform Obstruction;
    [SerializeField] private float zoomSpeed = 2f;
    private float ClampAngle(float angle, float min, float max) {
        if (angle < -180f) angle += 360f;
        if (angle > 180f) angle -= 360f;
        return Mathf.Clamp(angle, min, max);
    }

    private void Movement() {
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        if(Input.GetKey(KeyCode.LeftShift)) {
            flycam_current_speed = flycam_sprint_speed;
        }
        else {
            flycam_current_speed = flycam_normal_speed;
        }
        flycam.transform.Translate(input * flycam_current_speed * Time.deltaTime);
    }

    private void Rotation() {
        Vector3 mouseInput = new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0);
        flycam.transform.Rotate(mouseInput * flycam_sensitivity * Time.deltaTime);
        Vector3 eulerRotation = flycam.transform.rotation.eulerAngles;
        flycam.transform.rotation = Quaternion.Euler(eulerRotation.x, eulerRotation.y, 0);
    }

    private void ViewObstructed() {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, pivot.position - cam.transform.position, out hit, 4.5f)) {
            if (hit.collider.gameObject.tag != "Ball" && hit.collider.gameObject.tag != "Grass") { //something in the way 
                Obstruction = hit.transform;
                Obstruction.gameObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                
                if (Vector3.Distance(Obstruction.position, cam.transform.position) >= 3f && Vector3.Distance(cam.transform.position, pivot.position) >= 1.5f) {
                    cam.transform.Translate(Vector3.forward * zoomSpeed * Time.deltaTime);
                }
            }
            else {
                Obstruction.gameObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                if (Vector3.Distance(cam.transform.position, pivot.position) < 4.5f) {
                    cam.transform.Translate(Vector3.back * zoomSpeed * Time.deltaTime);
                }
            }
        }
    }

    void Start() {
        Obstruction = target;
        cam.enabled = true;
        flycam.enabled = false;
        if (!useOffsetValues) {
            offset = target.position - cam.transform.position;
        }

        pivot.position = target.position;
        pivot.parent = target;
        currentRotation = Quaternion.identity;
        rb = ball.GetComponent<Rigidbody>();
        lastRotation = pivot.rotation;
    }
    void Update() {
        if (Input.GetKeyUp(KeyCode.C) && !PauseScript.gamePaused) {
            if (cam.enabled) {
                RenderSettings.fog = false;
                cam.enabled = false;
                flycam.enabled = true;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else {
                RenderSettings.fog = true;
                flycam.enabled = false;
                cam.enabled = true;
            }
        }
        if (cam.enabled) {
            ViewObstructed();
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            if (rb.linearVelocity.magnitude < 0.05f && !PauseScript.gamePaused) {
                if (Input.GetMouseButton(1)) {
                    float horizontal = Input.GetAxis("Mouse X") * rotateSpeed;
                    float vertical = Input.GetAxis("Mouse Y") * rotateSpeed;
                    pivot.Rotate(0, horizontal, 0, Space.World);

                    float desiredYAngle = pivot.eulerAngles.y;
                    float desiredXAngle = pivot.eulerAngles.x - vertical;
                    desiredXAngle = ClampAngle(desiredXAngle, -15f, 60f);
                    pivot.rotation = Quaternion.Euler(desiredXAngle, desiredYAngle, 0);
                    currentRotation = pivot.rotation;
                    lastRotation = pivot.rotation;
                    cam.transform.position = target.position - (currentRotation * offset); 
                }
            }
            else {
                pivot.rotation = lastRotation; 
            }
            cam.transform.position = target.position - (currentRotation * offset); 
            cam.transform.LookAt(pivot);
            cam.fieldOfView -= Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
            cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, 20f, 100f);
        }
        if(flycam.enabled) {
            Movement();
            Rotation();
        }
    }
}
