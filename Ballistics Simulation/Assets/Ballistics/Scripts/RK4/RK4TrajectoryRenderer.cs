using System.Collections.Generic;
using UnityEngine;

namespace BallisticsSimulation
{
    [RequireComponent(typeof(LineRenderer))]
    public class RK4TrajectoryRenderer : MonoBehaviour
    {
        #region Fields
        [SerializeField] private RK4BallisticsHandler _rk4Calculator;
        [SerializeField] private bool _drawBaseVectors = true;
        
        private LineRenderer _lineRenderer;
        private List<Vector3> _corners = new List<Vector3>();
        [SerializeField] private Transform _gunOrigin;

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
            if (_rk4Calculator != null)
            {
                _corners.Clear();
                var corners = _rk4Calculator.Trajectory;
                for (int i = 0; i < corners.Count; i++)
                {
                    Vector3 origin = _gunOrigin.position;
                    origin.y = 0;
                    Vector3 newCorner = origin + _rk4Calculator.StraightVector * (float)corners[i].X + Vector3.up * (float)corners[i].Y + _rk4Calculator.RightVector * (float)corners[i].Z;

                    _corners.Add(newCorner);
                }
                _lineRenderer.positionCount = _corners.Count;
                _lineRenderer.SetPositions(_corners.ToArray());
            }
            else
            {
                Debug.LogWarning("You need to assign RK Handler!");
            }
        }

        private void DrawVectors()
        {
            if (_gunOrigin != null && _rk4Calculator != null && _drawBaseVectors)
            {
                Debug.DrawLine(_gunOrigin.position, _gunOrigin.position + _rk4Calculator.DirectionVector, Color.blue);
                Debug.DrawLine(_gunOrigin.position, _gunOrigin.position + Vector3.up, Color.yellow);

                Debug.DrawLine(_gunOrigin.position, _gunOrigin.position + _rk4Calculator.StraightVector, Color.red);
                Debug.DrawLine(_gunOrigin.position, _gunOrigin.position + _rk4Calculator.RightVector, Color.green);
            }
        }
        #endregion
    }
}