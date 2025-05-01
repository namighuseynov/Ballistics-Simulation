using System;
using System.Collections.Generic;
using UnityEngine;

namespace BallisticsSimulation
{
    public class RKF45BallisticsHandler : Handler
    {
        #region Fields
        [Header("Ballistics")]
        [SerializeField] private BallisticsProperties _ballisticsProps;

        [Header("RK45 (Fehlberg)")]
        public double initialStep = 0.1;   // start step h
        public double eps = 1e-4;  // local error
        public double hMin = 1e-4;  // min step
        public double hMax = 1.0;   // max step
        public int maxSteps = 10000; // iterations

        [Header("Wind")]
        [SerializeField] private WindZone _windZone;

        [Header("Debug Trajectory (world-space)")]
        public List<State> trajectory = new List<State>();

        private Vector3 _directionVector = Vector3.zero;  
        private Vector3 _straightVector = Vector3.zero;  
        private Vector3 _rightVector = Vector3.zero;
        #endregion

        #region Method
        private void FixedUpdate()
        {
            CalculateTrajectory();
        }

        private void CalculateTrajectory()
        {
            double angleRad = GetAngle();
            trajectory.Clear();

            State state = new State(
                0, 0, 0,
                _ballisticsProps.startSpeed * Math.Cos(angleRad),
                _ballisticsProps.startSpeed * Math.Sin(angleRad),
                0
            );
            trajectory.Add(new State(state));

            double h = initialStep;
            int counter = 0;

            while (counter < maxSteps && state.Y >= 0.0)
            {
                State k1 = Derivatives(state);

                State k2 = Derivatives(state.Add(
                           k1.Dot(h * 0.25)));

                State k3 = Derivatives(state.Add(
                           k1.Dot(h * 3.0 / 32.0)
                      .Add(k2.Dot(h * 9.0 / 32.0))));

                State k4 = Derivatives(state.Add(
                           k1.Dot(h * 1932.0 / 2197.0)
                      .Add(k2.Dot(h * -7200.0 / 2197.0))
                      .Add(k3.Dot(h * 7296.0 / 2197.0))));

                State k5 = Derivatives(state.Add(
                           k1.Dot(h * 439.0 / 216.0)
                      .Add(k2.Dot(h * -8.0))
                      .Add(k3.Dot(h * 3680.0 / 513.0))
                      .Add(k4.Dot(h * -845.0 / 4104.0))));

                State k6 = Derivatives(state.Add(
                           k1.Dot(h * -8.0 / 27.0)
                      .Add(k2.Dot(h * 2.0))
                      .Add(k3.Dot(h * -3544.0 / 2565.0))
                      .Add(k4.Dot(h * 1859.0 / 4104.0))
                      .Add(k5.Dot(h * -11.0 / 40.0))));

                State y4 = state.Add(
                           k1.Dot(h * 25.0 / 216.0)
                      .Add(k3.Dot(h * 1408.0 / 2565.0))
                      .Add(k4.Dot(h * 2197.0 / 4104.0))
                      .Add(k5.Dot(h * -1.0 / 5.0)));

                State y5 = state.Add(
                           k1.Dot(h * 16.0 / 135.0)
                      .Add(k3.Dot(h * 6656.0 / 12825.0))
                      .Add(k4.Dot(h * 28561.0 / 56430.0))
                      .Add(k5.Dot(h * -9.0 / 50.0))
                      .Add(k6.Dot(h * 2.0 / 55.0)));

                double err = y5.Sub(y4).Magnitude();

                if (err <= eps || h <= hMin)
                {
                    state = y5;                   
                    trajectory.Add(new State(state));
                    counter++;
                }

                double s = 0.9 * Math.Pow(eps / Math.Max(err, 1e-15), 0.2);
                h = Math.Clamp(h * s, hMin, hMax);

                if (h < hMin) break;          
            }
        }

        private State Derivatives(State s)
        {
            double m = Math.Max(_ballisticsProps.mass, 1e-6);


            Vector3 vWorld = _straightVector * (float)s.Vx +
                             Vector3.up * (float)s.Vy +
                             _rightVector * (float)s.Vz;


            Vector3 vRel = vWorld - (_ballisticsProps.useWind ? GetWind() : Vector3.zero);

            double vMag = vRel.magnitude;
            double dragFactor = _ballisticsProps.airDensity *
                                _ballisticsProps.dragCoefficent *
                                _ballisticsProps.area * 0.5;

            Vector3 dragAcc = _ballisticsProps.useDrag
                ? -(float)(dragFactor / m) * (float)vMag * vRel
                : Vector3.zero;

            Vector3 gravity = _ballisticsProps.useGravity
                ? new Vector3(0, -9.81f, 0)
                : Vector3.zero;

            Vector3 accWorld = gravity + dragAcc;


            double ax = Vector3.Dot(accWorld, _straightVector);
            double ay = Vector3.Dot(accWorld, Vector3.up);
            double az = Vector3.Dot(accWorld, _rightVector);

            return new State(
                s.Vx, s.Vy, s.Vz,
                ax, ay, az);
        }

        private double GetAngle()
        {
            switch (_ballisticsProps.axisDirection)
            {
                case AxisDirection.Forward: _directionVector = transform.forward; break;
                case AxisDirection.Right: _directionVector = transform.right; break;
                case AxisDirection.Up: _directionVector = transform.up; break;
            }

            _straightVector = new Vector3(_directionVector.x, 0, _directionVector.z).normalized;
            _rightVector = -Vector3.Cross(_straightVector, _directionVector);

            double angle = Vector3.Angle(_directionVector, _straightVector);
            return angle * Math.PI / 180.0;
        }

        private Vector3 GetWind()
        {
            if (_windZone == null) return Vector3.zero;

            Vector3 mainDir = _windZone.transform.forward;

            Vector3 turbDir = Quaternion.AngleAxis(
                UnityEngine.Random.Range(0f, 360f),
                Vector3.up) * mainDir;

            float windSpeed = _windZone.windMain;
            float turbulence = _windZone.windTurbulence * UnityEngine.Random.Range(-1f, 1f);

            return mainDir * windSpeed + turbDir * turbulence;
        }

        #endregion

        #region Properties
        public AxisDirection Direction => _ballisticsProps.axisDirection;
        public Vector3 DirectionVec => _directionVector;
        public Vector3 StraightVec => _straightVector;
        public Vector3 RightVec => _rightVector;
        #endregion

    }
}