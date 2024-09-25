using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public struct AirfoilAnimation
{
    public Airfoil airfoil;
    public GameObject Model;
    public float maxAngle;
    public bool useY;
    public bool useZ;
}

public class AirfoilAnimations : MonoBehaviour
{
    public AirfoilAnimation[] animations;
    
    void Update()
    {
        foreach (AirfoilAnimation animation in animations)
        {
            float angle = animation.maxAngle * animation.airfoil.input;
            if (animation.useY)
            {
                animation.Model.transform.localEulerAngles = new Vector3(animation.Model.transform.localEulerAngles.x, 
                    angle, animation.Model.transform.localEulerAngles.z);
            }else if (animation.useZ)
            {
                animation.Model.transform.localEulerAngles = new Vector3(animation.Model.transform.localEulerAngles.x,
                    animation.Model.transform.localEulerAngles.y, angle);
            }
            else
            {
                animation.Model.transform.localEulerAngles = new Vector3(angle, animation.Model.transform.localEulerAngles.y, animation.Model.transform.localEulerAngles.z);
            }
        }
    }
}
