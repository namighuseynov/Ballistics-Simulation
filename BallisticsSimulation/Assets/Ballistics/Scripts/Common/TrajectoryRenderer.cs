using System.Collections.Generic;
using UnityEngine;

namespace BallisticsSimulation
{
    [RequireComponent(typeof(LineRenderer))]
    public class TrajectoryRenderer : MonoBehaviour
    {
        #region Fields
        [SerializeField] private BallisticsHandler _handler;
        [SerializeField] private bool _drawBaseVectors = true;
        
        private LineRenderer _lineRenderer;
        private List<Vector3> _corners = new List<Vector3>();
        [SerializeField] private Transform _gunOrigin;

        #endregion

        #region Methods
        private void Start()
        {
            _handler = GetComponent<BallisticsHandler>();
            _lineRenderer = GetComponent<LineRenderer>();
            if (_handler == null)
            {
                _handler = FindAnyObjectByType<BallisticsHandler>();
            }
            
            if (_handler != null && _gunOrigin == null)
            {
                _gunOrigin = _handler.Origin;
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
            if (_handler != null)
            {
                _corners.Clear();
                var corners = _handler.Trajectory;
                for (int i = 0; i < corners.Count; i++)
                {
                    Vector3 origin = _gunOrigin.position;
                    origin.y = 0;
                    Vector3 newCorner = origin + _handler.StraightVector * (float)corners[i].X + Vector3.up * (float)corners[i].Y + _handler.RightVector * (float)corners[i].Z;

                    _corners.Add(newCorner);
                }
                _lineRenderer.positionCount = _corners.Count;
                _lineRenderer.SetPositions(_corners.ToArray());

                if (_corners.Count > 0) Debug.Log(corners[corners.Count - 1].X);
            }
            else
            {
                Debug.LogWarning("You need to assign RK Handler!");
            }
        }

        private void DrawVectors()
        {
            if (_gunOrigin != null && _handler != null && _drawBaseVectors)
            {
                Debug.DrawLine(_gunOrigin.position, _gunOrigin.position + _handler.DirectionVector, Color.blue);
                Debug.DrawLine(_gunOrigin.position, _gunOrigin.position + Vector3.up, Color.yellow);

                Debug.DrawLine(_gunOrigin.position, _gunOrigin.position + _handler.StraightVector, Color.red);
                Debug.DrawLine(_gunOrigin.position, _gunOrigin.position + _handler.RightVector, Color.green);
            }
        }
        #endregion
    }
}