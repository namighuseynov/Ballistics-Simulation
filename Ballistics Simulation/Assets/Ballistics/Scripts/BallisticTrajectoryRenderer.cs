using BallisticsSimulation;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ballistic trajectory line renderer
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class BallisticTrajectoryRenderer : MonoBehaviour
{
    #region Fields
    [Header("Renderer Settings")]
    [SerializeField] private Transform _projectileSpawnPoint;
    [SerializeField] private ProjectileProperties _projectileProps;
    [SerializeField] private BallisticsCalculator _ballCalculator;
    private Vector3 _forwardDirection;
    private Vector3 _strightDirection;
    private Vector3 _rightDirection;
    private BallisticSettings _ballisticSettings;
    

    [Header("Trajectory")]
    [SerializeField] private float timeStep = 0.2f;
    [SerializeField] private float maxTime = 100;
    private List<Vector3> _trajectoryPoints = new List<Vector3>();
    private float elapsedTime = 0;
    private LineRenderer _lineRenderer;
    #endregion

    #region Methods
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

    private void DrawBaseVectors()
    {
        // Forward direction (green)
        _forwardDirection = _projectileSpawnPoint.up;

        // Stright direction (blue)
        _strightDirection = _forwardDirection;
        _strightDirection.y = 0;
        _strightDirection.Normalize();

        // Right direction (red)
        _rightDirection = Vector3.Cross(_projectileSpawnPoint.up, Vector3.up).normalized;

        // Draw debug lines
        Debug.DrawLine(_projectileSpawnPoint.position, _projectileSpawnPoint.position + _forwardDirection, Color.green);
        Debug.DrawLine(_projectileSpawnPoint.position, _projectileSpawnPoint.position + _rightDirection, Color.red);
        Debug.DrawLine(_projectileSpawnPoint.position, _projectileSpawnPoint.position + _strightDirection, Color.blue);
    }

    private void UpdateTrajectory()
    {
        _trajectoryPoints.Clear();

        float theta = Mathf.Deg2Rad * (Vector3.Angle(_strightDirection, _forwardDirection));
        Vector3 startPosition = _projectileSpawnPoint.position;
        Vector3 startedVelocity = new Vector3(
            Convert.ToSingle(_projectileProps.StartingSpeed*Math.Cos(theta)),
            Convert.ToSingle(_projectileProps.StartingSpeed*Math.Sin(theta)),
            0
            );
        Vector3 velocity = Vector3.zero;
        //Vector3 windForces = Wind();
        Vector3 displacement = Vector3.zero;
        elapsedTime = 0f;
        while (elapsedTime <= maxTime)
        {
            //Calculating
            Vector3 drag = (_ballisticSettings.UseDrag ? _ballCalculator.CalculateDrag(displacement.y, velocity) : Vector3.zero)*_projectileProps.Weight;
            Vector3 acceleration = new Vector3(-drag.x, (Gravity()*_projectileProps.Weight) - drag.y, -drag.z);
            velocity = startedVelocity + acceleration * elapsedTime;
            displacement = startedVelocity * elapsedTime + 0.5f * acceleration * MathF.Pow(elapsedTime, 2);
            Vector3 newCoord = startPosition + _strightDirection * displacement.x + Vector3.up * displacement.y;
            if (newCoord.y < startPosition.y) break; // Stop if the projectile hits the ground
            _trajectoryPoints.Add(newCoord);
            elapsedTime += timeStep;
        }
        // Update the line renderer
        RenderTrajectory();
    }
    private float Gravity()
    {
        if (_ballisticSettings.UseGravity) return Physics.gravity.y;
        return 0;
    }
    private Vector3 Wind()
    {
        Vector3 Wind = Vector3.zero;
        if (_ballisticSettings.UseWindForce)
        {
            Vector3 windDirection = _ballisticSettings.Wind.transform.forward;
            var windVelocity = CalculateWindVelocity();
            float angleBetweenWindWeapon = Mathf.Deg2Rad * Vector3.SignedAngle(windDirection, _projectileSpawnPoint.forward, Vector3.up);
            float windForceForward = windVelocity.magnitude * Convert.ToSingle(Math.Sin(angleBetweenWindWeapon));
            float windForceRight = windVelocity.magnitude * Convert.ToSingle(Math.Cos(angleBetweenWindWeapon));
            Wind = new Vector3(windForceRight, 0, windForceForward);
        }
        return Wind;
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
    #endregion
}
