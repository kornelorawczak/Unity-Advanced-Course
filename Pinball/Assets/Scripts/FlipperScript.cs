using UnityEngine;

public class FlipperScript : MonoBehaviour {
    public float restPosition = 0f;
    public float pressedPosition = 45f;
    public float hitStrength = 10000f;
    public float flipperDamper = 150f;
    [SerializeField]
    KeyCode flipperKey;
    HingeJoint hinge;

    void Start () {
        hinge = GetComponent<HingeJoint>();
        hinge.useSpring = true;
    }

    void Update() {
        JointSpring spring = new JointSpring();
        spring.spring = hitStrength;
        spring.damper = flipperDamper;
        if (Input.GetKey(flipperKey)) {
            spring.targetPosition = pressedPosition;
        }
        else {
            spring.targetPosition = restPosition;
        }
        hinge.spring = spring;
        hinge.useLimits = true;
    
    }
}
