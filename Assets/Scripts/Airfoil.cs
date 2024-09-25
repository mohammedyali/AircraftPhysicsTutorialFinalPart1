using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AirfoilType
{
    none,
    flap,
    aleron,
    elevator,
    rudder

}

public class Airfoil : MonoBehaviour
{
    public AirfoilType _type;
    public float AvgChord = 1;
    public float Span = 2;
    public float offset = 2;

    [HideInInspector] public float input;
    [HideInInspector] public bool ShowGizmos;

    private Vector3 wind;

    float e = 0.89f;
    public float Cdv = 0.3f;

    public AnimationCurve Cl;

    public Vector3 Velocity;
    public Vector3 LocalVelocity;
    public float AoA;
    public float maxAngle = 20;
    [Range(-1,1)] public int multiplier;

    public void FlightPhysics(Rigidbody main, float airDensity, float _input, Vector3 _wind)
    {
        wind = _wind;
        CalculateState(main);
        input = _input;
        float aera = AvgChord * Span;
        float speed = LocalVelocity.z;
        float verticalSpeed = LocalVelocity.y;
        float AR = Span / AvgChord;
        float angle = AoA;
        float _Cl = Cl.Evaluate(angle + (Mathf.Clamp(input, -1, 1) * maxAngle * multiplier));
        float dynamicPressure = 0.5f * airDensity * (speed * speed);
        float lift = _Cl * dynamicPressure * aera;

        float Cdi = _Cl * _Cl;
        Cdi /= Mathf.PI * AR * e;
        float inducedDrag = Cdi * dynamicPressure * aera;
        float verticalDrag = 0;
        if(verticalSpeed != 0) verticalDrag = Cdv * (0.5f * airDensity * (verticalSpeed * verticalSpeed)) * aera * (-verticalSpeed / Mathf.Abs(verticalSpeed));

        Vector3 liftDirection = Vector3.Cross(Velocity.normalized, transform.right);
        Vector3 dragDirection = -Velocity.normalized;

        main.AddForceAtPosition(liftDirection * lift, transform.position);
        Debug.DrawRay(transform.position, liftDirection * (lift/main.mass), Color.white);

        main.AddForceAtPosition(dragDirection * inducedDrag, transform.position);
        Debug.DrawRay(transform.position, dragDirection * (inducedDrag / main.mass), Color.red);

        main.AddForceAtPosition(verticalDrag * transform.up, transform.position);
        Debug.DrawRay(transform.position, transform.up * (verticalDrag / main.mass), Color.red);
    }

    private void OnDrawGizmos()
    {
        if(ShowGizmos)
            DrawFoilShape();
    }

    public void DrawFoilShape()
    {
        Vector3 Point1 = new Vector3(-Span/2, 0, AvgChord/2 + offset);
        Vector3 Point2 = new Vector3(Span/2, 0, AvgChord/2 + offset);
        Vector3 Point3 = new Vector3(Span/2, 0, -AvgChord/2 + offset);
        Vector3 Point4 = new Vector3(-Span/2, 0, -AvgChord/2 + offset);

        Point1 = transform.TransformPoint(Point1);
        Point2 = transform.TransformPoint(Point2);
        Point3 = transform.TransformPoint(Point3);
        Point4 = transform.TransformPoint(Point4);

        Debug.DrawLine(Point1, Point2, Color.white);
        Debug.DrawLine(Point2, Point3, Color.white);
        Debug.DrawLine(Point3, Point4, Color.white);
        Debug.DrawLine(Point4, Point1, Color.white);
        Debug.DrawLine(Point2, Point4, Color.white);
    }

    public void CalculateState(Rigidbody main)
    {
        var invRotation = Quaternion.Inverse(transform.rotation);
        Velocity = main.GetPointVelocity(transform.position) + wind;
        LocalVelocity = invRotation * Velocity;
        CalculateAngleOfAttack();
    }
    void CalculateAngleOfAttack()
    {
        AoA = Mathf.Atan2(-LocalVelocity.y, LocalVelocity.z) * Mathf.Rad2Deg;
    }
}
