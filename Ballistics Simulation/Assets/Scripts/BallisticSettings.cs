using UnityEngine;

public class BallisticSettings : MonoBehaviour
{
    public bool useGravity = true;
    public bool useDrag = false;
    public bool useWindForce = false;
    public bool useThrust = false;

    public Vector3 windDirection = Vector3.zero;
    public Vector3 thrustDirection = Vector3.zero;
    public float projectileForce = 0;

    public float AtmosphereTemperature;
    public float AtmospherePressure;
    public float AtmosphereDensity;
}