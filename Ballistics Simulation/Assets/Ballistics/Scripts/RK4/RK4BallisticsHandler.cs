using System;
using System.Collections.Generic;
using UnityEngine;

namespace BallisticsSimulation
{
    public class RK4BallisticsHandler : Handler
    {
        #region Fields
        [Header("Ballistics")]
        [SerializeField] private BallisticsProperties _ballisticsProps;
        [SerializeField] private Transform _origin;

        [Header("RK4")]
        [SerializeField] private bool _runtimeCalculate = true;
        [SerializeField] private double stepSize = 0.1f;
        [SerializeField] private int maxSteps = 10000;

        [Header("Trajectory (read‑only)")]
        private readonly List<State> _trajectory = new();
        private Vector3 _directionVector = Vector3.zero;
        private Vector3 _straightVector = Vector3.zero;
        private Vector3 _rightVector = Vector3.zero;

        [Header("Wind")]
        [SerializeField] private WindZone _windZone;
        #endregion

        #region Unity loop

        private void Start()
        {
            if (_windZone == null)
            {
                _windZone = FindAnyObjectByType<WindZone>();
            }
        }
        private void FixedUpdate()
        {
            if (_runtimeCalculate)
                Recalculate();
        }
        #endregion

        #region Public API
        public IReadOnlyList<State> Trajectory => _trajectory;

        public double StepSize => stepSize;

        public Vector3 StraightVector => _straightVector;
        public Vector3 RightVector => _rightVector;
        public Vector3 DirectionVector => _directionVector;

        public void Recalculate() => CalculateTrajectory();
        #endregion

        #region Core math
        private void CalculateTrajectory()
        {
            double angleRad = GetAngle();
            int counter = 0;
            _trajectory.Clear();

            State state = new State(
                0, _origin.position.y, 0,
                _ballisticsProps.startSpeed * Math.Cos(angleRad),
                _ballisticsProps.startSpeed * Math.Sin(angleRad),
                0
            );
            _trajectory.Add(new State(state));

            while (counter < maxSteps && state.Y >= 0.0)
            {
                State k1 = Derivatives(state);
                State k2 = Derivatives(state.Add(k1.Dot(stepSize * 0.5)));
                State k3 = Derivatives(state.Add(k2.Dot(stepSize * 0.5)));
                State k4 = Derivatives(state.Add(k3.Dot(stepSize)));

                State delta = (k1.Add(k2.Dot(2))
                                 .Add(k3.Dot(2))
                                 .Add(k4))
                               .Dot(stepSize / 6.0);

                state = state.Add(delta);
                _trajectory.Add(new State(state));
                counter++;
            }
        }

        private State Derivatives(State s)
        {
            if (_ballisticsProps.mass == 0) _ballisticsProps.mass = 0.001;

            Vector3 vWorld = _straightVector * (float)s.Vx
                           + Vector3.up * (float)s.Vy
                           + _rightVector * (float)s.Vz;

            Vector3 vRel = vWorld - (_ballisticsProps.useWind ? GetWind() : Vector3.zero);

            double vMag = vRel.magnitude;
            double dragFactor = _ballisticsProps.airDensity
                              * _ballisticsProps.dragCoefficent
                              * _ballisticsProps.area * 0.5;

            Vector3 dragAcc = _ballisticsProps.useDrag
                            ? -(float)(dragFactor / _ballisticsProps.mass) * (float)vMag * vRel
                            : Vector3.zero;

            Vector3 gravity = _ballisticsProps.useGravity ? new Vector3(0, -9.81f, 0) : Vector3.zero;
            Vector3 accWorld = gravity + dragAcc;

            double ax = Vector3.Dot(accWorld, _straightVector);
            double ay = Vector3.Dot(accWorld, Vector3.up);
            double az = Vector3.Dot(accWorld, _rightVector);

            return new State(s.Vx, s.Vy, s.Vz, ax, ay, az);
        }
        #endregion

        #region Helpers
        private double GetAngle()
        {
            _directionVector = Direction switch
            {
                AxisDirection.Right => _origin.right,
                AxisDirection.Up => _origin.up,
                _ => _origin.forward,
            };

            _straightVector = new Vector3(_directionVector.x, 0, _directionVector.z).normalized;
            _rightVector = -Vector3.Cross(_straightVector, _directionVector);

            int sign = MathF.Sign(_directionVector.y);

            double angle = Vector3.Angle(_directionVector, _straightVector);
            return angle * Mathf.Deg2Rad * sign;
        }

        private Vector3 GetWind()
        {
            Vector3 main = _windZone.transform.forward;

            Vector3 turbDir = Quaternion.AngleAxis(
                                  UnityEngine.Random.Range(0f, 360f),
                                  Vector3.up) * main;

            float windSpeed = _windZone.windMain;
            float turbulenceAbs = _windZone.windTurbulence
                                * UnityEngine.Random.Range(-1f, 1f);

            return main * windSpeed + turbDir * turbulenceAbs;
        }
        #endregion

        #region Properties
        public AxisDirection Direction => _ballisticsProps.axisDirection;
        #endregion
    }
}
