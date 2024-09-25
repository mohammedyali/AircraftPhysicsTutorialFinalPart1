using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : MonoBehaviour
{
    public Rigidbody main;
    public float EngineForce; //in Newtons
    public float Throttle;

    public GameObject prop;
    // Update is called once per frame
    void FixedUpdate()
    {
        main.AddForceAtPosition(transform.forward * EngineForce * Throttle, transform.position);
        if(prop != null) { prop.transform.Rotate(0, 1000* Throttle, 0, Space.Self); }
    }
}
