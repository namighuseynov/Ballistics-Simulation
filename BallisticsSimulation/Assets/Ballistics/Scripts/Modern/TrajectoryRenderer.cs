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
        private Vector3[] _positions = new Vector3[0];

        #endregion

        #region Methods
        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            if (_handler == null) _handler = GetComponent<BallisticsHandler>();
            if (_handler == null) _handler = FindAnyObjectByType<BallisticsHandler>();
        }
        private void Update()
        {
            if (_lineRenderer == null || _handler == null) return;

            RenderTrajectory();

            if (_drawBaseVectors) DrawDebugVectors();
        }
        private void RenderTrajectory()
        {
            var trajectory = _handler.Trajectory;
            int count = trajectory.Count;

            if (count < 2)
            {
                _lineRenderer.positionCount = 0;
                return;
            }

            if (_positions.Length != count)
            {
                _positions = new Vector3[count];
            }

            for (int i = 0; i < count; i++)
            {
                _positions[i] = trajectory[i].Position;
            }

            _lineRenderer.positionCount = count;
            _lineRenderer.SetPositions(_positions);
        }

        private void DrawDebugVectors()
        {
            Transform origin = _handler.Origin;
            if (origin == null) return;

            Debug.DrawRay(origin.position, origin.forward * 2f, Color.blue);

            Debug.DrawRay(origin.position, Vector3.up * 2f, Color.yellow);
        }
        #endregion
    }
}