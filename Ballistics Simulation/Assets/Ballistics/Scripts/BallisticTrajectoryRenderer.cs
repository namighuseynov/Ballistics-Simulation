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
    private List<Vector3> coords = new List<Vector3>();
    private float elapsedTime = 0;
    private LineRenderer lineRenderer;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (!_ballisticSettings)
        {
            _ballisticSettings = GetComponent<BallisticSettings>();
        }
        
        // Initialize line renderer
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.positionCount = 0;
    }

    private void DrawBaseVectors()
    {
        // Forward basis (green)
        forwardDirection = _projectileSpawnPoint.position + _projectileSpawnPoint.up;

        // Up basis (blue)
        upDirection = _projectileSpawnPoint.up;
        upDirection.y = 0;
        upDirection.Normalize();
        // Right basis (red)
        rightDirection = Vector3.Cross(_projectileSpawnPoint.up, Vector3.up).normalized;

        // Draw debug lines
        Debug.DrawLine(_projectileSpawnPoint.position, forwardDirection, Color.green);
        Debug.DrawLine(_projectileSpawnPoint.position, _projectileSpawnPoint.position + rightDirection, Color.red);
        Debug.DrawLine(_projectileSpawnPoint.position, _projectileSpawnPoint.position + upDirection, Color.blue);
    }

    private void UpdateCoords()
    {
        elapsedTime = 0f;
        coords.Clear();
        float theta = (90 - transform.eulerAngles.z) * Mathf.PI / 180;
        Vector3 startPosition = _projectileSpawnPoint.position;
        Vector3 launchDirection = upDirection;
        float v0 = _projectileProps.StartingSpeed;
        float v0x = v0 * Mathf.Cos(theta);
        float v0y = v0 * Mathf.Sin(theta);
        Vector3 velocity = new Vector3(v0x, v0y);

        while (elapsedTime <= maxTime)
        {
            float dragX = Drag(velocity.x);
            float dragY = Drag(velocity.y);

            velocity.x = v0x - dragX * elapsedTime;
            velocity.y = v0y + Physics.gravity.y * elapsedTime * _projectileProps.Weight - dragY * elapsedTime;

            float Sx = v0x * elapsedTime - Drag(velocity[0]) * Mathf.Pow(elapsedTime, 2) * 0.5f;
            float Sy = v0y * elapsedTime + (0.5f * Physics.gravity.y * Mathf.Pow(elapsedTime, 2) * _projectileProps.Weight) - Drag(velocity[1]) * Mathf.Pow(elapsedTime, 2) * 0.5f;


            Vector3 newCoord = startPosition + launchDirection.normalized*Sx + Vector3.up*Sy;

            if (newCoord.y < startPosition.y) break; // Stop if the projectile hits the ground

            coords.Add(newCoord);
            elapsedTime += timeStep;
        }
        Debug.Log(Vector3.Distance(coords[coords.Count - 1], startPosition));
        // Update the line renderer
        lineRenderer.positionCount = coords.Count;
        lineRenderer.SetPositions(coords.ToArray());
    }

    private float Drag(float speed)
    {
        return _projectileProps.Density * Mathf.Pow(speed, 2) * _projectileProps.Weight * _projectileProps.Area * _projectileProps.dragCoefficient * 0.5f;
    }

    private void Update()
    {
        DrawBaseVectors();
        UpdateCoords();
    }
}
