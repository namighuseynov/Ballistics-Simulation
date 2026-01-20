using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Detects collisions along a ballistic trajectory.
/// </summary>
public class TrajectoryCollisionDetector : MonoBehaviour
{
    #region Fields
    [Header("Settings")]
    [SerializeField] private BallisticTrajectoryRenderer _trajectoryRenderer;
    [SerializeField] private LayerMask collisionLayer; // Define which layers to check for collisions
    [SerializeField] private float detectionRadius = 0.1f; // Radius for collision detection around trajectory points

    private List<Vector3> _trajectoryPoints = new List<Vector3>();
    #endregion

    #region Methods

    private void Start()
    {
        if (!_trajectoryRenderer)
        {
            _trajectoryRenderer = GetComponent<BallisticTrajectoryRenderer>();
        }
    }
    private void Update()
    {
        DetectCollisions();
    }

    private void DetectCollisions()
    {
        // Get the trajectory points from the renderer
        _trajectoryPoints = _trajectoryRenderer.TrajectoryPoints;

        // Iterate through trajectory points
        foreach (var point in _trajectoryPoints)
        {
            // Perform a sphere cast to detect collisions
            Collider[] hitColliders = Physics.OverlapSphere(point, detectionRadius, collisionLayer);
            if (hitColliders.Length > 0)
            {
                foreach (var collider in hitColliders)
                {
                    HandleCollision(collider, point);
                }
                break; // Stop further checks after the first collision
            }
        }
    }

    private void HandleCollision(Collider collider, Vector3 collisionPoint)
    {
        Debug.Log($"Collision detected with {collider.gameObject.name} at {collisionPoint}");
        // Add additional collision handling logic here
    }
    #endregion
}