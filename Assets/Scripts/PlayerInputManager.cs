using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    public Aircraft aircraft;
    public bool isActive;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    float flaps;
    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            aircraft.yaw = Input.GetAxis("Yaw");
            aircraft.pitch = Input.GetAxis("Vertical");
            aircraft.roll = Input.GetAxis("Horizontal");

            float flapTarget = 0;
            if (Input.GetKey(KeyCode.F)) { flapTarget = 1; }
            else { flapTarget = 0; }
            flaps = Mathf.Lerp(flaps, flapTarget, Time.deltaTime /2);
            aircraft.flaps = flaps;
        }
    }
}
