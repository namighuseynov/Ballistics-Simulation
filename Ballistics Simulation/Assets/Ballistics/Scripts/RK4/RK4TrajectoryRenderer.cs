using System.Collections.Generic;
using UnityEngine;

namespace BallisticsSimulation
{
    [RequireComponent(typeof(LineRenderer))]
    public class RK4TrajectoryRenderer : MonoBehaviour
    {
        #region Fields
        [SerializeField] private RK4BallisticsHandler _calculator;
        private LineRenderer _lineRenderer;
        private List<Vector3> _corners = new List<Vector3>();

        #endregion

        #region Methods
        private void Start()
        {
            _calculator = GetComponent<RK4BallisticsHandler>();
            _lineRenderer = GetComponent<LineRenderer>();
            if (_calculator == null)
            {
                _calculator = GameObject.FindAnyObjectByType<RK4BallisticsHandler>();
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
            var corners = _calculator.Trajectories;
            for (int i = 0; i < corners.Count; i++)
            {
                Vector3 newCorner = _calculator.StraghtVector* (float)corners[i].X + Vector3.up* (float)corners[i].Y + Vector3.right* (float)corners[i].Z;

                _corners.Add(newCorner);
            }
            _lineRenderer.positionCount = _corners.Count;
            _lineRenderer.SetPositions(_corners.ToArray());

            Debug.Log("Distance: " + corners[corners.Count - 1].X);
        }

        private void DrawVectors()
        {
            Debug.DrawLine(_calculator.transform.position, _calculator.DirectionVector, Color.blue);
            Debug.DrawLine(_calculator.transform.position, Vector3.up, Color.yellow);

            Debug.DrawLine(_calculator.transform.position, _calculator.StraghtVector, Color.red);
            Debug.DrawLine(_calculator.transform.position, _calculator.RightVector, Color.green);
        }
        #endregion
    }
}