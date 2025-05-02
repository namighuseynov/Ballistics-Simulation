using System.Collections.Generic;
using UnityEngine;

namespace BallisticsSimulation
{
    [RequireComponent(typeof(LineRenderer))]
    public class RK4TrajectoryRenderer : MonoBehaviour
    {
        #region Fields
        [SerializeField] private RK4BallisticsHandler _rk4Calculator;
        
        private LineRenderer _lineRenderer;
        private List<Vector3> _corners = new List<Vector3>();

        #endregion

        #region Methods
        private void Start()
        {
            _rk4Calculator = GetComponent<RK4BallisticsHandler>();
            _lineRenderer = GetComponent<LineRenderer>();
            if (_rk4Calculator == null)
            {
                _rk4Calculator = FindAnyObjectByType<RK4BallisticsHandler>();
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
            var corners = _rk4Calculator.Trajectory;
            for (int i = 0; i < corners.Count; i++)
            {
                Vector3 newCorner = _rk4Calculator.StraightVector * (float)corners[i].X + Vector3.up* (float)corners[i].Y + _rk4Calculator.RightVector* (float)corners[i].Z;

                _corners.Add(newCorner);
            }
            _lineRenderer.positionCount = _corners.Count;
            _lineRenderer.SetPositions(_corners.ToArray());
        }

        private void DrawVectors()
        {
            Debug.DrawLine(_rk4Calculator.transform.position, _rk4Calculator.DirectionVector, Color.blue);
            Debug.DrawLine(_rk4Calculator.transform.position, Vector3.up, Color.yellow);

            Debug.DrawLine(_rk4Calculator.transform.position, _rk4Calculator.StraightVector, Color.red);
            Debug.DrawLine(_rk4Calculator.transform.position, _rk4Calculator.RightVector, Color.green);
        }
        #endregion
    }
}