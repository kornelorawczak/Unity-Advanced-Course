using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathScript : MonoBehaviour
{
    void Start()
    {
        
    }

    void OnTriggerEnter(Collider other) {
        other.transform.position = BallScript.lastPosition;
        Rigidbody rb = other.GetComponent<Rigidbody>();
        rb.linearVelocity = new Vector3(0, 0, 0);
    }

    void Update()
    {
        
    }
}
