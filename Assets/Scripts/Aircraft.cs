using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aircraft : MonoBehaviour
{
    public Rigidbody main;
    public Vector3 CM;

    public Vector3 Wind;

    public Vector3 Velocity;
    public Vector3 LocalVelocity;
    public Vector3 AngularVelocity;

    [Range(-1, 1)] public float pitch;
    [Range(-1, 1)] public float roll;
    [Range(-1, 1)] public float yaw;
    [Range(0, 1)] public float flaps;
    [Range(0, 1)] public float throttle;
    [Range(0, 1)] public float brakes;

    public Airfoil[] airfoils;
    public Engine[] engines;
    public WheelCollider[] wheels;

    public float AirDensity;
    public float AirViscocity = 0; // in Pa
    public float AirPressure = 101300; // in Pa
    public float CSG = 287.05f; // assuming air is dry
    public float AirTemp = 298; //in K

    public Vector3 AircraftBodyAera;
    public Vector3 Cbd; // coefficient of body drag

    public bool showGizmos;

    // Start is called before the first frame update
    void Start()
    {
        main.centerOfMass = CM;
    }

    void FixedUpdate()
    {
        CalculateState();
        ApplyAirfoilForce();
    }

    public void ApplyAirfoilForce()
    {
        foreach (Airfoil airfoil in airfoils)
        {
            if(airfoil._type == AirfoilType.elevator)
                airfoil.FlightPhysics(main, AirDensity, pitch, Wind);
            if (airfoil._type == AirfoilType.aleron)
                airfoil.FlightPhysics(main, AirDensity, roll, Wind);
            if (airfoil._type == AirfoilType.rudder)
                airfoil.FlightPhysics(main, AirDensity, yaw, Wind);
            if (airfoil._type == AirfoilType.flap)
                airfoil.FlightPhysics(main, AirDensity, flaps, Wind);
            if (airfoil._type == AirfoilType.none)
                airfoil.FlightPhysics(main, AirDensity, 0, Wind);
        }

        foreach (Engine engine in engines)
        {
            engine.Throttle = throttle;
        }

        foreach (WheelCollider wheel in wheels)
        {
            wheel.brakeTorque = 10000 * brakes;
            if(brakes == 0) { wheel.brakeTorque = 0; }
        }

            float xDrag = 0;
        if (LocalVelocity.x != 0) { xDrag = Cbd.x * (0.5f * AirDensity * (LocalVelocity.x * LocalVelocity.x) * AircraftBodyAera.x * (-LocalVelocity.x/Mathf.Abs(LocalVelocity.x))); }
        float yDrag = 0;
        if (LocalVelocity.y != 0) { yDrag = Cbd.y * (0.5f * AirDensity * (LocalVelocity.y * LocalVelocity.y) * AircraftBodyAera.y * (-LocalVelocity.y / Mathf.Abs(LocalVelocity.y))); }
        float zDrag = 0;
        if (LocalVelocity.z != 0) { zDrag = Cbd.z * (0.5f * AirDensity * (LocalVelocity.z * LocalVelocity.z) * AircraftBodyAera.z * (-LocalVelocity.z / Mathf.Abs(LocalVelocity.z))); }

        Vector3 drag = new Vector3(xDrag, yDrag, zDrag);

        main.AddForce(transform.TransformVector(drag));
        Debug.DrawRay(transform.position, transform.up * drag.y * (1 / main.mass), Color.red);
        Debug.DrawRay(transform.position, transform.right * drag.x * (1 / main.mass), Color.red);
        Debug.DrawRay(transform.position, transform.forward * drag.z * (1/main.mass), Color.red);
    }

    public void CalculateState()
    {
        var invRotation = Quaternion.Inverse(main.rotation);
        Velocity = main.velocity;
        LocalVelocity = invRotation * Velocity;
        AirDensity = (AirPressure) / (CSG * AirTemp);
    }

    private void OnDrawGizmos()
    {
        foreach (Airfoil airfoil in airfoils)
        {
            airfoil.ShowGizmos = showGizmos;
        }
    }
}
