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
    [Header("Settings")]
    [SerializeField] private Transform _projectileSpawnPoint;
    [SerializeField] private ProjectileProperties _projectileProps;
    [SerializeField] private BallisticsCalculator _ballCalculator;
    private BallisticSettings _ballisticSettings;
    [Header("Directions")]
    private Vector3 _forwardDirection;
    private Vector3 _strightDirection;
    private Vector3 _rightDirection;
    private Vector3 _upDirection;

    [Header("Trajectory")]
    [SerializeField] private float timeStep = 0.2f;
    [SerializeField] private float maxTime = 100;
    public List<Vector3> TrajectoryPoints = new List<Vector3>();
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

    private void DrawDirectionVectors()
    {
        // Forward direction (green)
        _forwardDirection = _projectileSpawnPoint.up;

        // Stright direction (blue)
        _strightDirection = _forwardDirection;
        _strightDirection.y = 0;
        _strightDirection.Normalize();

        // Right direction (red)
        _rightDirection = Vector3.Cross(_projectileSpawnPoint.up, Vector3.up).normalized;

        // Up direction (yellow)
        _upDirection = Vector3.up;

        // Draw debug lines
        Debug.DrawLine(_projectileSpawnPoint.position, _projectileSpawnPoint.position + _forwardDirection, Color.green);
        Debug.DrawLine(_projectileSpawnPoint.position, _projectileSpawnPoint.position + _rightDirection, Color.red);
        Debug.DrawLine(_projectileSpawnPoint.position, _projectileSpawnPoint.position + _strightDirection, Color.blue);
        Debug.DrawLine(_projectileSpawnPoint.position, _projectileSpawnPoint.position + _upDirection, Color.yellow);
    }

    private void UpdateTrajectory()
    {
        TrajectoryPoints.Clear();

        float theta = Vector3.Angle(_strightDirection, _forwardDirection);

        Vector3 startPosition = _projectileSpawnPoint.position;
        Vector3 startedVelocity = new Vector3(
            _projectileProps.StartingSpeed * MathF.Cos(Mathf.Deg2Rad * theta),
            _projectileProps.StartingSpeed * MathF.Sin(Mathf.Deg2Rad * theta),
            0
        );
        Vector3 velocity = startedVelocity;
        Vector3 displacement = Vector3.zero;
        elapsedTime = 0f;
        float fuelMass = _projectileProps.FuelMass;
        float totalMass = fuelMass + _projectileProps.Weight;
        float mDot = fuelMass / _projectileProps.FuelBurningTime;
        while (elapsedTime <= maxTime)
        {
            // Forces
            Vector3 drag = _ballisticSettings.UseDrag
            ? _ballCalculator.CalculateDrag(displacement.y, velocity) * totalMass
            : Vector3.zero;

            Vector3 gravity = _ballisticSettings.UseGravity
                ? _ballCalculator.CalculateGravity(displacement.y) * totalMass
                : Vector3.zero;

            Vector3 wind = _ballisticSettings.UseWindForce
                ? _ballCalculator.CalculateWind(_strightDirection) * totalMass
                : Vector3.zero;

            Vector3 thrust = fuelMass > 0 && _ballisticSettings.UseThrust
                ? _ballCalculator.CalculateThrust(_forwardDirection, fuelMass, _projectileProps.GasOutflowSpeed) * totalMass
                : Vector3.zero;
            //Debug.Log("Elapsed time" + elapsedTime + " " + "Speed" + velocity.magnitude);
            // Acceleration
            Vector3 acceleration = new Vector3(
                drag.x + wind.x + thrust.x,
                drag.y + gravity.y + thrust.y,
                drag.z - wind.z + thrust.z
            );

            // Mass update
            fuelMass = Mathf.Max(0, (fuelMass - mDot*timeStep));
            //Debug.Log($"Time: {elapsedTime:0.00}s, Fuel Mass: {fuelMass:0.0000}kg");
            velocity += acceleration * timeStep;
            displacement += velocity * timeStep + acceleration * MathF.Pow(timeStep, 2) * 0.5f;
            Vector3 newCoord = startPosition + _strightDirection * displacement.x + Vector3.up * displacement.y + _rightDirection * displacement.z;
            if (newCoord.y < 0) break; // Stop if the projectile hits the ground
            TrajectoryPoints.Add(newCoord);
            elapsedTime += timeStep;
        } 
        // Update the line renderer
        Debug.Log(Vector3.Distance(startPosition, TrajectoryPoints[TrajectoryPoints.Count - 1]));
        RenderTrajectory();
    }

    private void RenderTrajectory()
    {
        _lineRenderer.positionCount = TrajectoryPoints.Count;
        _lineRenderer.SetPositions(TrajectoryPoints.ToArray());
    }
    private void Update()
    {
        DrawDirectionVectors();
        UpdateTrajectory();
    }
    #endregion
}