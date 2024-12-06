using BallisticsSimulation;
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
    private const float L = -0.0065f;   // (K/m)
    private const float R = 8.31447f;   // (J/(mol*K))
    private const float M = 0.0289647f; // (kg/mol)
    private const float EarthRadius = 6371000f; // Earth radius 
    #endregion

    public float CalculateDrag(float height, float speed)
    {
        float density = CalculateDensity(height);
        float drag = _projectileProperties.dragCoefficient * density * Mathf.Pow(speed, 2) * _projectileProperties.Area * 0.5f;
        return drag;
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
        float power = (-Mathf.Abs(CalculateGravity(height)) * M) / (R * L);

        float var = 1 + (L * height / _atmosphereProperties.Temperature);
        return _atmosphereProperties.Pressure * Mathf.Pow(var, power);
    }

    public float CalculateGravity(float height)
    {
        return 9.80665f * Mathf.Pow(EarthRadius / (EarthRadius + height), 2) * -1;
    }
}
