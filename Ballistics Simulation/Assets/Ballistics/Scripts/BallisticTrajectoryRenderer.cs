using BallisticsSimulation;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BallisticTrajectoryRenderer : MonoBehaviour
{
    private Vector3 forwardDirection;
    private Vector3 upDirection;
    private Vector3 rightDirection;
    [SerializeField] private Transform _projectileSpawnPoint;
    [SerializeField] private ProjectileProperties _projectileProps;
    [SerializeField] private BallisticSettings _ballisticSettings;

    [Header("Trajectory")]
    [SerializeField] private float timeStep = 0.2f;
    [SerializeField] private float maxTime = 100;
    private List<Vector3> _trajectoryPoints = new List<Vector3>();
    private float elapsedTime = 0;
    private LineRenderer _lineRenderer;


    private void Start()
    {
        InitializeLineRenderer();
        ValidateSettings();
    }

    private void InitializeLineRenderer()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.startWidth = 0.05f;
        _lineRenderer.endWidth = 0.05f;
        _lineRenderer.positionCount = 0;
    }

    private void ValidateSettings()
    {
        if (!_ballisticSettings)
        {
            _ballisticSettings = GetComponent<BallisticSettings>();
        }
    }

    private Vector3 CalculateWindVelocity()
    {
        Vector3 windDirection = _ballisticSettings.Wind.transform.forward;
        float windSpeed = _ballisticSettings.Wind.windMain;
        float turbulence = _ballisticSettings.Wind.windTurbulence * UnityEngine.Random.Range(-1f, 1f);

        return windDirection * (windSpeed + turbulence);
    }

    private Vector3 CalculateWindForces()
    {
        Vector3 windDirection = _ballisticSettings.Wind.transform.forward;
        var windVelocity = CalculateWindVelocity();
        float angleBetweenWindWeapon = Vector3.SignedAngle(windDirection, _projectileSpawnPoint.forward, Vector3.up) * MathF.PI / 180;
        float windForceForward = windVelocity.magnitude * Convert.ToSingle(Math.Sin(angleBetweenWindWeapon));
        float windForceRight = windVelocity.magnitude * Convert.ToSingle(Math.Cos(angleBetweenWindWeapon));

        return new Vector3(windForceRight, 0, windForceForward);
    }

    private void DrawBaseVectors()
    {
        // Forward basis (green)
        forwardDirection = _projectileSpawnPoint.up;

        // Up basis (blue)
        upDirection = _projectileSpawnPoint.up;
        upDirection.y = 0;
        upDirection.Normalize();
        // Right basis (red)
        rightDirection = Vector3.Cross(_projectileSpawnPoint.up, Vector3.up).normalized;

        // Draw debug lines
        Debug.DrawLine(_projectileSpawnPoint.position, _projectileSpawnPoint.position + forwardDirection, Color.green);
        Debug.DrawLine(_projectileSpawnPoint.position, _projectileSpawnPoint.position + rightDirection, Color.red);
        Debug.DrawLine(_projectileSpawnPoint.position, _projectileSpawnPoint.position + upDirection, Color.blue);
    }

    private void UpdateTrajectory()
    {
        _trajectoryPoints.Clear();

        float theta = Mathf.Deg2Rad * (Vector3.Angle(upDirection, forwardDirection));
        Vector3 startPosition = _projectileSpawnPoint.position;
        Vector3 launchDirection = upDirection;
        float v0 = _projectileProps.StartingSpeed;
        float v0x = v0 * Mathf.Cos(theta);
        float v0y = v0 * Mathf.Sin(theta);
        Vector3 velocity = new Vector3(v0x, v0y);
        Vector3 windForces = CalculateWindForces();
        
        elapsedTime = 0f;
        while (elapsedTime <= maxTime)
        {
            float dragX = Drag(velocity.x);
            float dragY = Drag(velocity.y);

            velocity.x = v0x - dragX * elapsedTime;
            velocity.y = v0y + Physics.gravity.y * elapsedTime * _projectileProps.Weight - dragY * elapsedTime;
            velocity.z = 0;

            float Sx = v0x * elapsedTime - Drag(velocity.x) * Mathf.Pow(elapsedTime, 2) * 0.5f + windForces.z*elapsedTime;
            float Sy = v0y * elapsedTime + (0.5f * Physics.gravity.y * Mathf.Pow(elapsedTime, 2) * _projectileProps.Weight) - Drag(velocity.y) * Mathf.Pow(elapsedTime, 2) * 0.5f;
            float Sz = windForces.x * elapsedTime;

            Vector3 newCoord = startPosition + launchDirection.normalized*Sx + Vector3.up*Sy + _projectileSpawnPoint.forward * Sz;

            if (newCoord.y < startPosition.y) break; // Stop if the projectile hits the ground

            _trajectoryPoints.Add(newCoord);
            elapsedTime += timeStep;
        }
        // Update the line renderer
        RenderTrajectory();
    }


    private float Drag(float speed)
    {
        return _projectileProps.Density * Mathf.Pow(speed, 2) * _projectileProps.Weight * _projectileProps.Area * _projectileProps.dragCoefficient * 0.5f;
    }

    private void RenderTrajectory()
    {
        _lineRenderer.positionCount = _trajectoryPoints.Count;
        _lineRenderer.SetPositions(_trajectoryPoints.ToArray());
    }

    private void Update()
    {
        DrawBaseVectors();
        UpdateTrajectory();
    }
}
