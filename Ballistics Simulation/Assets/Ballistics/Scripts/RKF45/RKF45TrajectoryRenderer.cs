using System.Collections.Generic;
using UnityEngine;

namespace BallisticsSimulation
{
    [RequireComponent(typeof(LineRenderer))]
    public class RKF45TrajectoryRenderer : MonoBehaviour
    {
        #region Fields
        [SerializeField] private RKF45BallisticsHandler _rk45FCalculator;

        private LineRenderer _lineRenderer;
        private List<Vector3> _corners = new List<Vector3>();

        #endregion

        #region Methods
        private void Start()
        {
            _rk45FCalculator = GetComponent<RKF45BallisticsHandler>();
            _lineRenderer = GetComponent<LineRenderer>();
            if (_rk45FCalculator == null)
            {
                _rk45FCalculator = FindAnyObjectByType<RKF45BallisticsHandler>();
            }
        }
        private void Update()
        {
            if (_lineRenderer != null)
            {
                DrawVectors();
                RenderTrajectory();
            }
        }
        private void RenderTrajectory()
        {
            _corners.Clear();
            var corners = _rk45FCalculator.trajectory;
            for (int i = 0; i < corners.Count; i++)
            {
                Vector3 newCorner = _rk45FCalculator.StraightVec * (float)corners[i].X + Vector3.up * (float)corners[i].Y + _rk45FCalculator.RightVec * (float)corners[i].Z;

                _corners.Add(newCorner);
            }
            _lineRenderer.positionCount = _corners.Count;
            _lineRenderer.SetPositions(_corners.ToArray());

            Debug.Log("Distance: " + corners[corners.Count - 1].X);
        }

        private void DrawVectors()
        {
            Debug.DrawLine(_rk45FCalculator.transform.position, _rk45FCalculator.DirectionVec, Color.blue);
            Debug.DrawLine(_rk45FCalculator.transform.position, Vector3.up, Color.yellow);

            Debug.DrawLine(_rk45FCalculator.transform.position, _rk45FCalculator.StraightVec, Color.red);
            Debug.DrawLine(_rk45FCalculator.transform.position, _rk45FCalculator.RightVec, Color.green);
        }
        #endregion
    }
}