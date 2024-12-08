using BallisticsSimulation;
using System;
using UnityEngine;

/// <summary>
/// Ballistics calculator
/// </summary>
public class BallisticsCalculator : MonoBehaviour
{
    #region Fields
    //Ballistic settings
    [Header("Ballistics settings")]
    [SerializeField] private AtmosphereProperties _atmosphereProperties;
    [SerializeField] private ProjectileProperties _projectileProperties;

    [Header("Wind")]
    [SerializeField] private WindZone _wind; //Wind object
    private const float L = -0.0065f;   // (K/m)
    private const float R = 8.31447f;   // (J/(mol*K))
    private const float M = 0.0289647f; // (kg/mol)
    private const float EarthRadius = 6371000f; // Earth radius 
    #endregion

    #region Methods
    public Vector3 CalculateDrag(float height, Vector3 velocity)
    {
        float density = CalculateDensity(height); 

        float dragX = CalculateDragComponent(velocity.x, density);
        float dragY = CalculateDragComponent(velocity.y, density);
        float dragZ = CalculateDragComponent(velocity.z, density);

        return new Vector3(dragX, dragY, dragZ);
    }

    private float CalculateDragComponent(float velocityComponent, float density)
    {
        float speedSquared = MathF.Pow(velocityComponent, 2); 
        float direction = velocityComponent >= 0 ? -1f : 1f; 
        return direction * _projectileProperties.dragCoefficient * density * speedSquared * _projectileProperties.Area * 0.5f;
    }

    private float CalculateTemperature(float height)
    {
        return Mathf.Max(_atmosphereProperties.Temperature + height * L, 1f); 
    }

    private float CalculateDensity(float height)
    {
        float temperature = CalculateTemperature(height);
        float pressure = CalculatePressure(height);

        return (pressure * M) / (R * temperature);
    }

    private float CalculatePressure(float height)
    {
        float power = (-Mathf.Abs(CalculateGravity(height).y) * M) / (R * L);

        float var = 1 + (L * height / _atmosphereProperties.Temperature);
        return _atmosphereProperties.Pressure * Mathf.Pow(var, power);
    }

    public Vector3 CalculateGravity(float height)
    {
        return -9.80665f * Mathf.Pow(EarthRadius / (EarthRadius + height), 2) * Vector3.up;
    }
    public Vector3 CalculateWind(Vector3 direction)
    {
        Vector3 windDirection = _wind.transform.forward;
        float windSpeed = _wind.windMain;
        float turbulence = _wind.windTurbulence * UnityEngine.Random.Range(-1f, 1f);
        Vector3 windVelocity = windDirection * (windSpeed + turbulence);

        float angle = Mathf.Deg2Rad * Vector3.SignedAngle(windDirection, direction, Vector3.up);
        float windForceForward = windVelocity.magnitude * Convert.ToSingle(Math.Sin(angle));
        float windForceRight = windVelocity.magnitude * Convert.ToSingle(Math.Cos(angle));
        return new Vector3(windForceForward, 0, windForceRight);
    }
    #endregion
}
